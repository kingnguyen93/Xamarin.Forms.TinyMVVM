using System;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.Forms.TinyMVVM
{
    public class ViewModelCoreMethods : IViewModelCoreMethods
    {
        private Page currentPage;
        private TinyViewModel currentViewModel;

        public ViewModelCoreMethods(Page currentPage, TinyViewModel viewModel)
        {
            this.currentPage = currentPage;
            currentViewModel = viewModel;
        }

        public async Task DisplayAlert(string title, string message, string cancel)
        {
            if (currentPage != null)
                await currentPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            if (currentPage != null)
                return await currentPage.DisplayAlert(title, message, accept, cancel);
            return false;
        }

        public async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            if (currentPage != null)
                return await currentPage.DisplayActionSheet(title, cancel, destruction, buttons);
            return null;
        }

        public async Task PushPage<TPage>(bool modal = false, bool animate = true) where TPage : Page
        {
            TPage page = TinyIOC.Container.Resolve<TPage>();
            await PushPage(page, modal, animate);
        }

        public async Task PushPage(Type pageType, bool modal = false, bool animate = true)
        {
            var page = TinyIOC.Container.Resolve(pageType) as Page;
            await PushPage(page, modal, animate);
        }

        public async Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            await TinyIOC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PushPage(page, modal, animate);
        }

        public async Task PushViewModel<T>(bool modal = false, bool animate = true) where T : TinyViewModel
        {
            T pageModel = TinyIOC.Container.Resolve<T>();
            await PushPageModel(pageModel, modal, animate);
        }

        public async Task PushViewModel<T>(object data, bool modal = false, bool animate = true) where T : TinyViewModel
        {
            T pageModel = TinyIOC.Container.Resolve<T>();
            await PushPageModel(pageModel, data, modal, animate);
        }

        public async Task PushViewModel<T>(NavigationParameters parameters, bool modal = false, bool animate = true) where T : TinyViewModel
        {
            T pageModel = TinyIOC.Container.Resolve<T>();
            await PushPageModel(pageModel, parameters, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIOC.Container.Resolve(viewModelType) as TinyViewModel;
            return PushPageModel(viewModel, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, object data, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIOC.Container.Resolve(viewModelType) as TinyViewModel;
            return PushPageModel(viewModel, data, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIOC.Container.Resolve(viewModelType) as TinyViewModel;
            return PushPageModel(viewModel, parameters, modal, animate);
        }

        private async Task PushPageModel(TinyViewModel viewModel, bool modal = false, bool animate = true)
        {
            var page = ViewModelResolver.ResolveViewModel(viewModel);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        private async Task PushPageModel(TinyViewModel viewModel, object data, bool modal = false, bool animate = true)
        {
            var page = ViewModelResolver.ResolveViewModel(viewModel, data);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        private async Task PushPageModel(TinyViewModel viewModel, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            viewModel.Parameters = parameters;
            var page = ViewModelResolver.ResolveViewModel(viewModel, parameters: parameters);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public async Task PushViewModel<T, TPage>(bool modal = false, bool animate = true) where T : TinyViewModel where TPage : Page
        {
            T viewModel = TinyIOC.Container.Resolve<T>();
            TPage page = TinyIOC.Container.Resolve<TPage>();
            ViewModelResolver.BindingPageModel(page, viewModel);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public async Task PushViewModel<T, TPage>(object data, bool modal = false, bool animate = true) where T : TinyViewModel where TPage : Page
        {
            T viewModel = TinyIOC.Container.Resolve<T>();
            TPage page = TinyIOC.Container.Resolve<TPage>();
            ViewModelResolver.BindingPageModel(page, viewModel, data);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public async Task PushViewModel<T, TPage>(NavigationParameters parameters, bool modal = false, bool animate = true) where T : TinyViewModel where TPage : Page
        {
            var viewModel = TinyIOC.Container.Resolve<T>() as TinyViewModel;
            TPage page = TinyIOC.Container.Resolve<TPage>();
            ViewModelResolver.BindingPageModel(page, viewModel, parameters: parameters);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, Type pageType, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIOC.Container.Resolve(viewModelType) as TinyViewModel;
            var page = TinyIOC.Container.Resolve(pageType) as Page;
            ViewModelResolver.BindingPageModel(page, viewModel);
            return PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, Type pageType, object data, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIOC.Container.Resolve(viewModelType) as TinyViewModel;
            var page = TinyIOC.Container.Resolve(pageType) as Page;
            ViewModelResolver.BindingPageModel(page, viewModel, data);
            return PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, Type pageType, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIOC.Container.Resolve(viewModelType) as TinyViewModel;
            var page = TinyIOC.Container.Resolve(pageType) as Page;
            ViewModelResolver.BindingPageModel(page, viewModel, parameters: parameters);
            return PushPageModelWithPage(page, viewModel, modal, animate);
        }

        private async Task PushPageModelWithPage(Page page, TinyViewModel viewodel, bool modal = false, bool animate = true)
        {
            viewodel.PreviousViewModel = currentViewModel; //This is the previous page model because it's push to a new one, and this is current
            viewodel.CurrentNavigationServiceName = currentViewModel.CurrentNavigationServiceName;

            if (string.IsNullOrWhiteSpace(viewodel.PreviousNavigationServiceName))
                viewodel.PreviousNavigationServiceName = currentViewModel.PreviousNavigationServiceName;

            viewodel.IsModal = modal;

            await TinyIOC.Container.Resolve<INavigationService>(viewodel.CurrentNavigationServiceName).PushPage(page, modal, animate);
        }

        public async Task PopViewModel(bool modal = false, bool animate = true)
        {
            if (currentViewModel.IsModalFirstChild)
            {
                await PopModalNavigationService(animate);
            }
            else
            {
                if (modal)
                    currentViewModel.RaisePageWasPopped();

                await TinyIOC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PopPage(modal, animate);
            }
        }

        public async Task PopToRoot(bool animate)
        {
            await TinyIOC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PopToRoot(animate);
        }

        public void RemoveFromNavigation()
        {
            currentViewModel.RaisePageWasPopped();
            currentPage.Navigation.RemovePage(currentPage);
        }

        public void RemoveFromNavigation<T>(bool removeAll = false) where T : TinyViewModel
        {
            //var pages = currentPage.Navigation.Where (o => o is T);
            foreach (var page in currentPage.Navigation.NavigationStack.Reverse().ToList())
            {
                if (page.BindingContext is T)
                {
                    page.GetModel()?.RaisePageWasPopped();
                    currentPage.Navigation.RemovePage(page);
                    if (!removeAll)
                        break;
                }
            }
        }

        public async Task<string> PushViewModelWithNewNavigation<T>(object data, bool animate = true) where T : TinyViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>();
            var navigationName = Guid.NewGuid().ToString();
            var naviationContainer = new NavigationContainer(page, navigationName);
            await PushNewNavigationServiceModal(naviationContainer, page.GetModel(), animate);
            return navigationName;
        }

        public Task PushNewNavigationServiceModal(TabbedNavigationContainer tabbedNavigationContainer, TinyViewModel baseViewModel = null, bool animate = true)
        {
            var models = tabbedNavigationContainer.TabbedPages.Select(o => o.GetModel()).ToList();
            if (baseViewModel != null)
                models.Add(baseViewModel);
            return PushNewNavigationServiceModal(tabbedNavigationContainer, models.ToArray(), animate);
        }

        public Task PushNewNavigationServiceModal(INavigationService newNavigationService, TinyViewModel basePageModels, bool animate = true)
        {
            return PushNewNavigationServiceModal(newNavigationService, new TinyViewModel[] { basePageModels }, animate);
        }

        public Task PushNewNavigationServiceModal(MasterDetailNavigationContainer masterDetailContainer, TinyViewModel basePageModel = null, bool animate = true)
        {
            var models = masterDetailContainer.Pages.Select(o =>
            {
                if (o.Value is NavigationPage)
                    return ((NavigationPage)o.Value).CurrentPage.GetModel();
                else
                    return o.Value.GetModel();
            }).ToList();

            if (basePageModel != null)
                models.Add(basePageModel);

            return PushNewNavigationServiceModal(masterDetailContainer, models.ToArray(), animate);
        }

        public async Task PushNewNavigationServiceModal(INavigationService newNavigationService, TinyViewModel[] basePageModels, bool animate = true)
        {
            if (!(newNavigationService is Page navPage))
                throw new Exception("Navigation service is not Page");

            foreach (var pageModel in basePageModels)
            {
                pageModel.CurrentNavigationServiceName = newNavigationService.NavigationServiceName;
                pageModel.PreviousNavigationServiceName = currentViewModel.CurrentNavigationServiceName;
                pageModel.IsModalFirstChild = true;
            }

            await TinyIOC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PushPage(navPage, true, animate);
        }

        public async Task PopModalNavigationService(bool animate = true)
        {
            var currentNavigationService = TinyIOC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName);
            currentNavigationService.NotifyChildrenPageWasPopped();

            TinyIOC.Container.Unregister<INavigationService>(currentViewModel.CurrentNavigationServiceName);

            var navServiceName = currentViewModel.PreviousNavigationServiceName;
            await TinyIOC.Container.Resolve<INavigationService>(navServiceName).PopPage(animate);
        }

        public void SwitchOutRootNavigation(string navigationServiceName)
        {
            INavigationService rootNavigation = TinyIOC.Container.Resolve<INavigationService>(navigationServiceName);

            if (!(rootNavigation is Page))
                throw new Exception("Navigation service is not a page");

            Application.Current.MainPage = rootNavigation as Page;
        }

        public Task<TinyViewModel> SwitchSelectedRootPageModel<T>() where T : TinyViewModel
        {
            return TinyIOC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).SwitchSelectedRootViewModel<T>();
        }

        public Task<TinyViewModel> SwitchSelectedTab<T>() where T : TinyViewModel
        {
            return SwitchSelectedRootPageModel<T>();
        }

        public Task<TinyViewModel> SwitchSelectedMaster<T>() where T : TinyViewModel
        {
            return SwitchSelectedRootPageModel<T>();
        }

        public void BatchBegin()
        {
            currentPage.BatchBegin();
        }

        public void BatchCommit()
        {
            currentPage.BatchCommit();
        }
    }
}