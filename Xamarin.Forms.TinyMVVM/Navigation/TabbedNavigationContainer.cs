using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TinyMVVM
{
    public class TabbedNavigationContainer : TabbedPage, INavigationService
    {
        private List<Page> _tabs = new List<Page>();
        public IEnumerable<Page> TabbedPages { get => _tabs; }

        public TabbedNavigationContainer() : this(Constants.DefaultNavigationServiceName)
        {
        }

        public TabbedNavigationContainer(string navigationServiceName)
        {
            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
        }

        protected void RegisterNavigation()
        {
            TinyIoC.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        public virtual Page AddTab<T>(string title, string icon, object data = null) where T : TinyViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>(data);
            page.GetModel().CurrentNavigationServiceName = NavigationServiceName;
            _tabs.Add(page);
            var navigationContainer = CreateContainerPageSafe(page);
            navigationContainer.Title = title;
            if (!string.IsNullOrWhiteSpace(icon))
                navigationContainer.Icon = icon;
            Children.Add(navigationContainer);
            return navigationContainer;
        }

        internal Page CreateContainerPageSafe(Page page)
        {
            if (page is NavigationPage || page is MasterDetailPage || page is TabbedPage)
                return page;

            return CreateContainerPage(page);
        }

        protected virtual Page CreateContainerPage(Page page)
        {
            return new NavigationPage(page);
        }

        public Task PushPage(Page page, TinyViewModel model, bool modal = false, bool animate = true)
        {
            if (modal)
                return CurrentPage.Navigation.PushModalAsync(CreateContainerPageSafe(page));
            return CurrentPage.Navigation.PushAsync(page);
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

        public string NavigationServiceName { get; private set; }

        public void NotifyChildrenPageWasPopped()
        {
            foreach (var page in this.Children)
            {
                if (page is NavigationPage)
                    ((NavigationPage)page).NotifyAllChildrenPopped();
            }
        }

        public Task<TinyViewModel> SwitchSelectedRootViewModel<T>() where T : TinyViewModel
        {
            var page = _tabs.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);

            if (page > -1)
            {
                CurrentPage = this.Children[page];
                var topOfStack = CurrentPage.Navigation.NavigationStack.LastOrDefault();
                if (topOfStack != null)
                    return Task.FromResult(topOfStack.GetModel());
            }
            return null;
        }
    }
}