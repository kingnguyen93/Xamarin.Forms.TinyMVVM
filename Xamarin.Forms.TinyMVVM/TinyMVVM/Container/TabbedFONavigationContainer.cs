using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TinyMVVM.Extensions;
using TinyMVVM.IoC;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace TinyMVVM
{
    /// <summary>
    /// This Tabbed navigation container for when you only want the tabs to appear on the first page and then push to a second page without tabs
    /// </summary>
    public class TabbedFONavigationContainer : NavigationPage, INavigationService
    {
        public string NavigationServiceName { get; private set; }

        public Xamarin.Forms.TabbedPage FirstTabbedPage { get; }

        private readonly List<Page> tabs = new List<Page>();

        public IEnumerable<Page> TabbedPages { get => tabs; }

        public TabbedFONavigationContainer(string titleOfFirstTab, bool bottomToolBar = false) : this(titleOfFirstTab, bottomToolBar, Constants.DefaultNavigationServiceName)
        {
        }

        public TabbedFONavigationContainer(string titleOfFirstTab, bool bottomToolBar, string navigationServiceName) : base(new Xamarin.Forms.TabbedPage())
        {
            FirstTabbedPage = (Xamarin.Forms.TabbedPage)CurrentPage;
            FirstTabbedPage.Title = titleOfFirstTab;

            if (bottomToolBar)
                FirstTabbedPage.On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            if (RootPage.GetModel() is TinyViewModel viewModel)
            {
                viewModel.CurrentNavigationServiceName = navigationServiceName;

                Device.BeginInvokeOnMainThread(() =>
                {
                    viewModel.OnNavigatedToAsync(new NavigationParameters());
                    viewModel.OnNavigatedTo(new NavigationParameters());
                });
            }

            if (RootPage is INavigatedAware aware)
            {
                aware.OnNavigatedToAsync(new NavigationParameters());
                aware.OnNavigatedTo(new NavigationParameters());
            }

            NavigationServiceName = navigationServiceName;
            RegisterNavigation();

            FirstTabbedPage.CurrentPageChanged += FirstTabbedPage_CurrentPageChanged;
        }

        private void FirstTabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {
            if (FirstTabbedPage.CurrentPage.Navigation.NavigationStack.LastOrDefault() is Page currentpage)
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
        }

        protected void RegisterNavigation()
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

        private Page AddTab(Page page, string title, string icon, NavigationParameters parameters = default)
        {
            tabs.Add(page);

            if (page.GetModel() is TinyViewModel viewModel)
            {
                viewModel.CurrentNavigationServiceName = NavigationServiceName;
            }

            var container = CreateContainerPageSafe(page);
            container.Title = title;
            container.IconImageSource = icon;

            FirstTabbedPage.Children.Add(container);

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
            return page;
        }

        public bool RemoveTab<T>() where T : TinyViewModel
        {
            if (CurrentPage == FirstTabbedPage)
            {
                var pageIndex = tabs.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);

                if (FirstTabbedPage.Children.Count > 0 && pageIndex > -1)
                {
                    if (pageIndex == FirstTabbedPage.Children.Count)
                        FirstTabbedPage.CurrentPage = FirstTabbedPage.Children[pageIndex - 1];

                    var page = FirstTabbedPage.Children[pageIndex];
                    tabs.Remove(page);
                    FirstTabbedPage.Children.Remove(page);

                    if (page.GetModel() is TinyViewModel viewModel)
                        viewModel.RaisePageWasPopped();

                    return true;
                }
                else
                {
                    throw new Exception("Cannot remove tabs when have only one tab");
                }
            }
            else
            {
                throw new Exception("Cannot remove tabs when the tab screen is not visible");
            }
        }

        public bool TryRemoveTab<T>() where T : TinyViewModel
        {
            try
            {
                return RemoveTab<T>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
                return false;
            }
        }

        public Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page));

            return Navigation.PushAsync(page);
        }

        public Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PopModalAsync(animate);

            return Navigation.PopAsync(animate);
        }

        public Task PopToRoot(bool animate = true)
        {
            return Navigation.PopToRootAsync(animate);
        }

        public void NotifyChildrenPageWasPopped()
        {
            foreach (var page in FirstTabbedPage.Children)
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
            if (CurrentPage == FirstTabbedPage)
            {
                var page = tabs.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);
                if (page > -1)
                {
                    FirstTabbedPage.CurrentPage = FirstTabbedPage.Children[page];

                    return Task.FromResult(FirstTabbedPage.CurrentPage.GetModel());
                }
            }
            else
            {
                throw new Exception("Cannot switch tabs when the tab screen is not visible");
            }

            return null;
        }
    }
}