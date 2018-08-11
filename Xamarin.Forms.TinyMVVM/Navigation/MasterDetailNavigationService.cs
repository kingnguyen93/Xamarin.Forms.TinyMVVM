using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TinyMVVM
{
    public class MasterDetailNavigationContainer : MasterDetailPage, INavigationService
    {
        public string NavigationServiceName { get; private set; }

        private ContentPage menuPage;
        private ListView listView = new ListView();
        private List<Page> pagesInner = new List<Page>();

        public Dictionary<string, Page> Pages { get; } = new Dictionary<string, Page>();

        protected ObservableCollection<string> PageNames { get; } = new ObservableCollection<string>();

        public MasterDetailNavigationContainer() : this(Constants.DefaultNavigationServiceName)
        {
        }

        public MasterDetailNavigationContainer(string navigationServiceName)
        {
            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
        }

        public void Init(string menuTitle, string menuIcon = null)
        {
            CreateMenuPage(menuTitle, menuIcon);
            RegisterNavigation();
        }

        protected virtual void RegisterNavigation()
        {
            TinyIoC.TinyIoC.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        protected virtual void CreateMenuPage(string menuPageTitle, string menuIcon = null)
        {
            listView.ItemsSource = PageNames;

            listView.ItemSelected += (sender, args) =>
            {
                if (Pages.ContainsKey((string)args.SelectedItem))
                {
                    Detail = Pages[(string)args.SelectedItem];
                }

                IsPresented = false;
            };

            menuPage = new ContentPage
            {
                Title = menuPageTitle,
                Content = listView
            };

            var navPage = new NavigationPage(menuPage) { Title = "Menu" };

            if (!String.IsNullOrEmpty(menuIcon))
                navPage.Icon = menuIcon;

            Master = navPage;
        }

        public virtual void AddPage<T>(string title, object data = null) where T : BaseViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>(data);
            page.GetModel().CurrentNavigationServiceName = NavigationServiceName;
            AddPage(page, title);
        }

        public virtual void AddPage(string modelName, string title, object data = null)
        {
            var pageModelType = Type.GetType(modelName);
            var page = ViewModelResolver.ResolveViewModel(pageModelType, data);
            page.GetModel().CurrentNavigationServiceName = NavigationServiceName;
            AddPage(page, title);
        }

        public virtual void AddPage(Type pageType, string title, object data = null)
        {
            var page = ViewModelResolver.ResolveViewModel(pageType, data);
            page.GetModel().CurrentNavigationServiceName = NavigationServiceName;
            AddPage(page, title);
        }

        private void AddPage(Page page, string title)
        {
            pagesInner.Add(page);
            var navigationContainer = CreateContainerPage(page);
            Pages.Add(title, navigationContainer);
            PageNames.Add(title);
            if (Pages.Count == 1)
                Detail = navigationContainer;
        }

        internal Page CreateContainerPageSafe(Page root)
        {
            if (root is NavigationPage || root is MasterDetailPage || root is TabbedPage)
                return root;

            return CreateContainerPage(root);
        }

        protected virtual Page CreateContainerPage(Page root)
        {
            return new NavigationPage(root);
        }

        public Task PushPage(Page page, BaseViewModel model, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page));
            return (Detail as NavigationPage).PushAsync(page, animate); //TODO: make this better
        }

        public Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page));
            return (Detail as NavigationPage).PushAsync(page, animate); //TODO: make this better
        }

        public Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PopModalAsync(animate);
            return (Detail as NavigationPage).PopAsync(animate); //TODO: make this better
        }

        public Task PopToRoot(bool animate = true)
        {
            return (Detail as NavigationPage).PopToRootAsync(animate);
        }

        public void NotifyChildrenPageWasPopped()
        {
            if (Master is NavigationPage)
                ((NavigationPage)Master).NotifyAllChildrenPopped();

            if (Master is INavigationService)
                ((INavigationService)Master).NotifyChildrenPageWasPopped();

            foreach (var page in Pages.Values)
            {
                if (page is NavigationPage)
                    ((NavigationPage)page).NotifyAllChildrenPopped();

                if (page is INavigationService)
                    ((INavigationService)page).NotifyChildrenPageWasPopped();
            }

            if (Pages != null && !Pages.ContainsValue(Detail) && Detail is NavigationPage)
                ((NavigationPage)Detail).NotifyAllChildrenPopped();

            if (Pages != null && !Pages.ContainsValue(Detail) && Detail is INavigationService)
                ((INavigationService)Detail).NotifyChildrenPageWasPopped();
        }

        public Task<BaseViewModel> SwitchSelectedRootViewModel<T>() where T : BaseViewModel
        {
            var tabIndex = pagesInner.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);

            listView.SelectedItem = PageNames[tabIndex];

            return Task.FromResult((Detail as NavigationPage).CurrentPage.GetModel());
        }
    }
}