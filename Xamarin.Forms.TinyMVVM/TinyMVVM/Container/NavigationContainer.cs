using System;
using System.Threading.Tasks;
using TinyMVVM.Extensions;
using TinyMVVM.IoC;

using Xamarin.Forms;

namespace TinyMVVM
{
    public class NavigationContainer : NavigationPage, INavigationService
    {
        public string NavigationServiceName { get; private set; }

        public NavigationContainer(Page root, NavigationParameters parameters = default)
            : this(root, Constants.DefaultNavigationServiceName, parameters)
        {
        }

        public NavigationContainer(Page root, string navigationPageName, NavigationParameters parameters = default)
            : base(root)
        {
            if (root.GetModel() is TinyViewModel rootViewModel)
            {
                rootViewModel.CurrentNavigationServiceName = navigationPageName;

                Device.BeginInvokeOnMainThread(() =>
                {
                    rootViewModel.OnNavigatedToAsync(parameters ?? new NavigationParameters()).ConfigureAwait(false);
                    rootViewModel.OnNavigatedTo(parameters ?? new NavigationParameters());
                });
            }

            if (root is INavigatedAware aware)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    aware.OnNavigatedToAsync(parameters ?? new NavigationParameters()).ConfigureAwait(false);
                    aware.OnNavigatedTo(parameters ?? new NavigationParameters());
                });
            }

            NavigationServiceName = navigationPageName;
            RegisterNavigation();
        }

        protected void RegisterNavigation()
        {
            _ = TinyIoCLocator.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        public Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page), animate);

            return Navigation.PushAsync(page, animate);
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