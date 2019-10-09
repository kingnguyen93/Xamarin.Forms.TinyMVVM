using System;
using TinyMVVM.IoC;

using Xamarin.Forms;

namespace TinyMVVM
{
    public static class ViewModelResolver
    {
        public static Page ResolveViewModel<TViewModel>() where TViewModel : TinyViewModel
        {
            return ResolveViewModel(typeof(TViewModel));
        }

        public static Page ResolveViewModel<TViewModel>(NavigationParameters parameters) where TViewModel : TinyViewModel
        {
            return ResolveViewModel(typeof(TViewModel), parameters);
        }

        public static Page ResolveViewModel<TViewModel, TPage>() where TViewModel : TinyViewModel where TPage : Page
        {
            return ResolveViewModel(typeof(TViewModel), typeof(TPage));
        }

        public static Page ResolveViewModel<TViewModel, TPage>(NavigationParameters parameters) where TViewModel : TinyViewModel where TPage : Page
        {
            return ResolveViewModel(typeof(TViewModel), typeof(TPage), parameters);
        }

        public static Page ResolveViewModel(Type viewModelType)
        {
            var pageType = ViewModelLocator.GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
                return null;
            return ResolveViewModel(viewModelType, pageType);
        }

        public static Page ResolveViewModel(Type viewModelType, NavigationParameters parameters)
        {
            var pageType = ViewModelLocator.GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
                return null;
            return ResolveViewModel(viewModelType, pageType, parameters);
        }

        public static Page ResolveViewModel(Type viewModelType, Type pageType)
        {
            var viewModel = TinyIoCLocator.Container.Resolve(viewModelType) as TinyViewModel;
            var page = TinyIoCLocator.Container.Resolve(pageType) as Page;
            return BindingPageModel(page, viewModel);
        }

        public static Page ResolveViewModel(Type viewModelType, Type pageType, NavigationParameters parameters)
        {
            var viewModel = TinyIoCLocator.Container.Resolve(viewModelType) as TinyViewModel;
            var page = TinyIoCLocator.Container.Resolve(pageType) as Page;
            return BindingPageModel(page, viewModel, parameters);
        }

        public static Page BindingPageModel(Page targetPage, TinyViewModel viewModel, NavigationParameters parameters = default)
        {
            viewModel.CurrentPage = targetPage;
            viewModel.CoreMethods = new ViewModelCoreMethods(targetPage, viewModel);
            viewModel.WireEvents();
            viewModel.InitializeAsync(parameters ?? new NavigationParameters());
            viewModel.Initialize(parameters ?? new NavigationParameters());
            targetPage.BindingContext = viewModel;
            viewModel.OnPageCreated(parameters ?? new NavigationParameters());

            return targetPage;
        }
    }
}