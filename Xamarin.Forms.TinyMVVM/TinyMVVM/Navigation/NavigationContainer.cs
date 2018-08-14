using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.TinyMVVM
{
    public class NavigationContainer : NavigationPage, INavigationService
    {
        public string NavigationServiceName { get; private set; }

        public NavigationContainer(Page root)
            : this(root, Constants.DefaultNavigationServiceName)
        {
        }

        public NavigationContainer(Page root, string navigationPageName)
            : base(root)
        {
            var viewModel = root.GetModel();
            if (viewModel != null)
            {
                viewModel.CurrentNavigationServiceName = navigationPageName;
                viewModel.OnPushed();
            }
            NavigationServiceName = navigationPageName;
            RegisterNavigation();
            Pushed += NavigationContainer_Pushed;
            Popped += NavigationContainer_Popped;
        }

        protected void RegisterNavigation()
        {
            TinyIOC.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        private void NavigationContainer_Pushed(object sender, NavigationEventArgs e)
        {
            if (e.Page.GetModel() is TinyViewModel viewModel)
            {
                viewModel.OnPushed();
            }
        }

        private void NavigationContainer_Popped(object sender, NavigationEventArgs e)
        {
            if (e.Page.GetModel() is TinyViewModel viewModel)
            {
                viewModel.OnPopped();
            }
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

        public virtual Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page), animate);
            return Navigation.PushAsync(page, animate);
        }

        public Task PushPage(Page page, TinyViewModel model, bool modal = false, bool animate = true)
        {
            ViewModelResolver.BindingPageModel(page, model);
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page), animate);
            return Navigation.PushAsync(page, animate);
        }

        public virtual Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PopModalAsync(animate);
            return Navigation.PopAsync(animate);
        }

        public Task PopToRoot(bool animate = true)
        {
            return Navigation.PopToRootAsync(animate);
        }

        public Task<TinyViewModel> SwitchSelectedRootViewModel<T>() where T : TinyViewModel
        {
            throw new Exception("This navigation container has no selected roots, just a single root");
        }

        public void NotifyChildrenPageWasPopped()
        {
            this.NotifyAllChildrenPopped();
        }
    }
}