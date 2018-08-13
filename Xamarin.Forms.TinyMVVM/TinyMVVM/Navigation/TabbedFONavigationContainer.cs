using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Forms.TinyMVVM
{
    /// <summary>
    /// This Tabbed navigation container for when you only want the tabs to appear on the first page and then push to a second page without tabs
    /// </summary>
    public class TabbedFONavigationContainer : NavigationPage, INavigationService
    {
        private TabbedPage _innerTabbedPage;
        public TabbedPage FirstTabbedPage { get => _innerTabbedPage; }

        private List<Page> _tabs = new List<Page>();

        public IEnumerable<Page> TabbedPages { get => _tabs; }

        public TabbedFONavigationContainer(string titleOfFirstTab) : this(titleOfFirstTab, Constants.DefaultNavigationServiceName)
        {
        }

        public TabbedFONavigationContainer(string titleOfFirstTab, string navigationServiceName) : base(new TabbedPage())
        {
            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
            _innerTabbedPage = (TabbedPage)CurrentPage;
            _innerTabbedPage.Title = titleOfFirstTab;
        }

        protected void RegisterNavigation()
        {
            TinyIOC.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        public virtual Page AddTab<T>(string title, string icon, object data = null) where T : TinyViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>(data);
            return AddTab(page, title, icon);
        }

        public virtual Page AddTab(string modelName, string title, string icon, object data = null)
        {
            var pageModelType = Type.GetType(modelName);
            return AddTab(pageModelType, title, icon, data);
        }

        public virtual Page AddTab(Type pageType, string title, string icon, object data = null)
        {
            var page = ViewModelResolver.ResolveViewModel(pageType, data);
            return AddTab(page, title, icon);
        }

        private Page AddTab(Page page, string title, string icon)
        {
            var viewModel = page.GetModel();
            viewModel.CurrentNavigationServiceName = NavigationServiceName;
            _tabs.Add(page);
            var container = CreateContainerPageSafe(page);
            container.Title = title;
            if (!string.IsNullOrWhiteSpace(icon))
                container.Icon = icon;
            _innerTabbedPage.Children.Add(container);
            viewModel.OnPushed();

            return container;
        }

        internal Page CreateContainerPageSafe(Page page)
        {
            if (page is NavigationPage || page is MasterDetailPage || page is TabbedPage)
                return page;

            return CreateContainerPage(page);
        }

        protected virtual Page CreateContainerPage(Page page)
        {
            return page;
        }

        public Task PushPage(Page page, TinyViewModel model, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page));
            return Navigation.PushAsync(page);
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

        public string NavigationServiceName { get; private set; }

        public void NotifyChildrenPageWasPopped()
        {
            foreach (var page in _innerTabbedPage.Children)
            {
                if (page is NavigationPage)
                    ((NavigationPage)page).NotifyAllChildrenPopped();
            }
        }

        public Task<TinyViewModel> SwitchSelectedRootViewModel<T>() where T : TinyViewModel
        {
            if (CurrentPage == _innerTabbedPage)
            {
                var page = _tabs.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);
                if (page > -1)
                {
                    _innerTabbedPage.CurrentPage = this._innerTabbedPage.Children[page];
                    return Task.FromResult(_innerTabbedPage.CurrentPage.GetModel());
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