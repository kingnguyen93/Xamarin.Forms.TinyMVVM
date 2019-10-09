using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyMVVM.Extensions;
using TinyMVVM.IoC;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace TinyMVVM
{
    public class TabbedNavigationContainer : Xamarin.Forms.TabbedPage, INavigationService
    {
        public string NavigationServiceName { get; private set; }

        private readonly Dictionary<Page, (Color BarBackgroundColor, Color BarTextColor)> tabs = new Dictionary<Page, (Color BarBackgroundColor, Color TextColor)>();

        public IEnumerable<Page> TabbedPages { get => tabs.Keys; }

        public TabbedNavigationContainer(bool bottomToolBar = false) : this(Constants.DefaultNavigationServiceName, bottomToolBar)
        {
        }

        public TabbedNavigationContainer(string navigationServiceName, bool bottomToolBar = false)
        {
            if (bottomToolBar)
                On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
        }

        private void RegisterNavigation()
        {
            _ = TinyIoCLocator.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        public virtual Page AddTab<T>(string title, string icon, NavigationParameters parameters = default) where T : TinyViewModel
        {
            return AddTab(typeof(T), title, icon, parameters);
        }

        public virtual Page AddTab(Type pageType, string title, string icon, NavigationParameters parameters = default)
        {
            var page = ViewModelResolver.ResolveViewModel(pageType, parameters);
            return AddTab(page, title, icon, parameters);
        }

        public Page AddTab(Page page, string title, string icon, NavigationParameters parameters = default)
        {
            tabs.Add(page, (BarBackgroundColor, BarTextColor));

            if (page.GetModel() is TinyViewModel viewModel)
            {
                viewModel.CurrentNavigationServiceName = NavigationServiceName;
            }

            var container = CreateContainerPageSafe(page);
            container.Title = title;
            container.IconImageSource = icon;

            Children.Add(container);

            return container;
        }

        private Page CreateContainerPageSafe(Page page)
        {
            if (page is NavigationPage || page is MasterDetailPage || page is Xamarin.Forms.TabbedPage)
                return page;

            return CreateContainerPage(page);
        }

        protected virtual Page CreateContainerPage(Page page)
        {
            return new NavigationPage(page);
        }

        public bool RemoveTab<T>() where T : TinyViewModel
        {
            var pageIndex = TabbedPages.ToList().FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);

            if (Children.Count > 0 && pageIndex > -1)
            {
                if (pageIndex == Children.Count)
                    CurrentPage = Children[pageIndex - 1];

                var page = Children[pageIndex];
                tabs.Remove(page);
                Children.Remove(page);

                if (page.GetModel() is TinyViewModel viewModel)
                    viewModel.RaisePageWasPopped();

                return true;
            }
            else
            {
                throw new Exception("Cannot remove tabs when have only one tab");
            }
        }

        public bool TryRemoveTab<T>() where T : TinyViewModel
        {
            try
            {
                return RemoveTab<T>();
            }
            catch
            {
                return false;
            }
        }

        public Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            if (modal)
                return CurrentPage.Navigation.PushModalAsync(CreateContainerPageSafe(page));

            return CurrentPage.Navigation.PushAsync(page);
        }

        public Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                return CurrentPage.Navigation.PopModalAsync(animate);

            return CurrentPage.Navigation.PopAsync(animate);
        }

        public Task PopToRoot(bool animate = true)
        {
            return CurrentPage.Navigation.PopToRootAsync(animate);
        }

        public void NotifyChildrenPageWasPopped()
        {
            foreach (var page in Children)
            {
                if (page is NavigationPage navPage)
                {
                    navPage.NotifyAllChildrenPopped();
                }
                else if (page is ContentPage)
                {
                    if (page.GetModel() is TinyViewModel viewModel)
                        viewModel.RaisePageWasPopped();
                }
            }
        }

        public Task<TinyViewModel> SwitchSelectedRootViewModel<T>() where T : TinyViewModel
        {
            var page = tabs.ToList().FindIndex(o => o.Key.GetModel().GetType().FullName == typeof(T).FullName);

            if (page > -1)
            {
                CurrentPage = Children[page];

                var topOfStack = CurrentPage.Navigation.NavigationStack.LastOrDefault();
                if (topOfStack != null)
                    return Task.FromResult(topOfStack.GetModel());
            }
            return null;
        }

        protected override void OnCurrentPageChanged()
        {
            /*var color = _tabs.FirstOrDefault(x => x.Key.GetType().FullName == CurrentPage.GetType().FullName).Value;

            BarBackgroundColor = color.BarBackgroundColor;
            BarTextColor = color.BarTextColor;*/

            if (CurrentPage.Navigation.NavigationStack.LastOrDefault() is Page currentpage)
            {
                if (currentpage.BindingContext is TinyViewModel viewModel)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        viewModel.OnNavigatedToAsync(new NavigationParameters()).ConfigureAwait(false);
                        viewModel.OnNavigatedTo(new NavigationParameters());
                    });
                }
                if (currentpage is INavigatedAware aware)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        aware.OnNavigatedToAsync(new NavigationParameters()).ConfigureAwait(false);
                        aware.OnNavigatedTo(new NavigationParameters());
                    });
                }
            }

            base.OnCurrentPageChanged();
        }
    }
}