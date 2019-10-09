using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace TinyMVVM
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the <see cref="Prism.Mvvm.ViewModelLocationProvider"/>.
    /// </summary>
    public static class ViewModelLocator
    {
        /// <summary>
        /// Instructs Prism whether or not to automatically create an instance of a ViewModel using a convention, and assign the associated View's <see cref="Xamarin.Forms.BindableObject.BindingContext"/> to that instance.
        /// </summary>
        public static readonly BindableProperty AutowireViewModelProperty =
            BindableProperty.CreateAttached("AutowireViewModel", typeof(bool?), typeof(ViewModelLocator), null, propertyChanged: OnAutowireViewModelChanged);

        /// <summary>
        /// Instructs Prism to use a given page as the parent for a Partial View
        /// </summary>
        public static readonly BindableProperty AutowirePartialViewProperty =
            BindableProperty.CreateAttached("AutowirePartialView", typeof(Page), typeof(ViewModelLocator), null, propertyChanged: OnAutowirePartialViewChanged);

        internal static readonly BindableProperty PartialViewsProperty =
            BindableProperty.CreateAttached("PrismPartialViews", typeof(List<BindableObject>), typeof(ViewModelLocator), null);

        /// <summary>
        /// Gets the AutowireViewModel property value.
        /// </summary>
        /// <param name="bindable"></param>
        /// <returns></returns>
        public static bool? GetAutowireViewModel(BindableObject bindable)
        {
            return (bool?)bindable.GetValue(ViewModelLocator.AutowireViewModelProperty);
        }

        /// <summary>
        /// Sets the AutowireViewModel property value.  If <c>true</c>, creates an instance of a ViewModel using a convention, and sets the associated View's <see cref="Xamarin.Forms.BindableObject.BindingContext"/> to that instance.
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="value"></param>
        public static void SetAutowireViewModel(BindableObject bindable, bool? value)
        {
            bindable.SetValue(ViewModelLocator.AutowireViewModelProperty, value);
        }

        public static Page GetAutowirePartialView(BindableObject bindable)
        {
            return (Page)bindable.GetValue(AutowirePartialViewProperty);
        }

        public static void SetAutowirePartialView(BindableObject bindable, Page page)
        {
            bindable.SetValue(AutowirePartialViewProperty, page);
        }

        internal static List<BindableObject> GetPartialViews(this Page page)
        {
            return (List<BindableObject>)page.GetValue(PartialViewsProperty);
        }

        private static void OnAutowireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            bool? bNewValue = (bool?)newValue;
            if (bNewValue.HasValue && bNewValue.Value)
                ViewModelLocationProvider.AutoWireViewModelChanged(bindable, Bind);
        }

        private static void OnAutowirePartialViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue)
                return;

            if (oldValue is Page oldPage)
            {
                List<BindableObject> oldPartials = oldPage.GetPartialViews();
                oldPartials.Remove(bindable);
            }

            if (newValue is Page page)
            {
                // Add View to Views Collection for Page.
                List<BindableObject> partialViews = page.GetPartialViews();
                if (partialViews == null)
                {
                    partialViews = new List<BindableObject>();
                    page.SetValue(PartialViewsProperty, partialViews);
                }

                partialViews.Add(bindable);
                // Set Autowire Property
                if (bindable.GetValue(AutowireViewModelProperty) == null)
                {
                    bindable.SetValue(AutowireViewModelProperty, true);
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> of a View
        /// </summary>
        /// <param name="view">The View to set the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> on</param>
        /// <param name="viewModel">The object to use as the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> for the View</param>
        private static void Bind(object view, object viewModel)
        {
            if (view is BindableObject element)
                element.BindingContext = viewModel;
        }

        public static readonly Dictionary<Type, Type> ViewModelMappings = new Dictionary<Type, Type>();

        public static void Init()
        {
        }

        public static Type GetPageTypeForViewModel<TViewModel>() where TViewModel : TinyViewModel
        {
            return GetPageTypeForViewModel(typeof(TViewModel));
        }

        public static Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (ViewModelMappings.ContainsKey(viewModelType))
            {
                return ViewModelMappings[viewModelType];
            }
            else
            {
                var pageName = ViewModelMapper.GetPageTypeName(viewModelType);
                var pageType = Type.GetType(pageName);
                if (pageType == null)
                    Debug.WriteLine($"No map for ${viewModelType} was found on navigation mappings");
                return pageType;
            }
        }

        public static void Register<TViewModel, TPage>() where TViewModel : TinyViewModel where TPage : Page
        {
            Register(typeof(TViewModel), typeof(TPage));
        }

        public static void Register(Type viewModel, Type page)
        {
            if (!ViewModelMappings.ContainsKey(viewModel))
            {
                ViewModelMappings.Add(viewModel, page);
            }
        }
    }
}