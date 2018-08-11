using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TinyMVVM
{
    public class ViewModelCoreMethods : IViewModelCoreMethods
    {
        private Page currentPage;
        private BaseViewModel currentViewModel;

        public ViewModelCoreMethods(Page currentPage, BaseViewModel viewModel)
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
            TPage page = TinyIoC.TinyIoC.Container.Resolve<TPage>();
            await PushPage(page);
        }

        public async Task PushPage(Type pageType, bool modal = false, bool animate = true)
        {
            var page = TinyIoC.TinyIoC.Container.Resolve(pageType) as Page;
            await PushPage(page);
        }

        public async Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            await TinyIoC.TinyIoC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PushPage(page, modal, animate);
        }

        public async Task PushViewModel<T>(object data, bool modal = false, bool animate = true) where T : BaseViewModel
        {
            T pageModel = TinyIoC.TinyIoC.Container.Resolve<T>();
            await PushPageModel(pageModel, data, modal, animate);
        }

        public async Task PushViewModel<T>(NavigationParameters parameters, bool modal = false, bool animate = true) where T : BaseViewModel
        {
            T pageModel = TinyIoC.TinyIoC.Container.Resolve<T>();
            await PushPageModel(pageModel, parameters, modal, animate);
        }

        public async Task PushViewModel<T, TPage>(object data, bool modal = false, bool animate = true) where T : BaseViewModel where TPage : Page
        {
            T viewModel = TinyIoC.TinyIoC.Container.Resolve<T>();
            TPage page = TinyIoC.TinyIoC.Container.Resolve<TPage>();
            ViewModelResolver.BindingPageModel(page, viewModel, data);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public async Task PushViewModel<T, TPage>(NavigationParameters parameters, bool modal = false, bool animate = true) where T : BaseViewModel where TPage : Page
        {
            var viewModel = TinyIoC.TinyIoC.Container.Resolve<T>() as BaseViewModel;
            TPage page = TinyIoC.TinyIoC.Container.Resolve<TPage>();
            viewModel.Parameters = parameters;
            ViewModelResolver.BindingPageModel(page, viewModel, null);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, object data, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIoC.TinyIoC.Container.Resolve(viewModelType) as BaseViewModel;
            return PushPageModel(viewModel, data, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIoC.TinyIoC.Container.Resolve(viewModelType) as BaseViewModel;
            return PushPageModel(viewModel, parameters, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, Type pageType, object data, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIoC.TinyIoC.Container.Resolve(viewModelType) as BaseViewModel;
            var page = TinyIoC.TinyIoC.Container.Resolve(pageType) as Page;
            ViewModelResolver.BindingPageModel(page, viewModel, data);
            return PushPageModelWithPage(page, viewModel, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, Type pageType, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            var viewModel = TinyIoC.TinyIoC.Container.Resolve(viewModelType) as BaseViewModel;
            var page = TinyIoC.TinyIoC.Container.Resolve(pageType) as Page;
            viewModel.Parameters = parameters;
            ViewModelResolver.BindingPageModel(page, viewModel, null);
            return PushPageModelWithPage(page, viewModel, modal, animate);
        }

        private async Task PushPageModel(BaseViewModel viewModel, object data, bool modal = false, bool animate = true)
        {
            var page = ViewModelResolver.ResolveViewModel(viewModel, data);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        private async Task PushPageModel(BaseViewModel viewModel, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            viewModel.Parameters = parameters;
            var page = ViewModelResolver.ResolveViewModel(viewModel, null);
            await PushPageModelWithPage(page, viewModel, modal, animate);
        }

        private async Task PushPageModelWithPage(Page page, BaseViewModel viewodel, bool modal = false, bool animate = true)
        {
            viewodel.PreviousViewModel = currentViewModel; //This is the previous page model because it's push to a new one, and this is current
            viewodel.CurrentNavigationServiceName = currentViewModel.CurrentNavigationServiceName;

            if (string.IsNullOrWhiteSpace(viewodel.PreviousNavigationServiceName))
                viewodel.PreviousNavigationServiceName = currentViewModel.PreviousNavigationServiceName;

            await TinyIoC.TinyIoC.Container.Resolve<INavigationService>(viewodel.CurrentNavigationServiceName).PushPage(page, modal, animate);
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

                await TinyIoC.TinyIoC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PopPage(modal, animate);
            }
        }

        public async Task PopToRoot(bool animate)
        {
            await TinyIoC.TinyIoC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PopToRoot(animate);
        }

        public void RemoveFromNavigation()
        {
            currentViewModel.RaisePageWasPopped();
            currentPage.Navigation.RemovePage(currentPage);
        }

        public void RemoveFromNavigation<T>(bool removeAll = false) where T : BaseViewModel
        {
            //var pages = currentPage.Navigation.Where (o => o is T);
            foreach (var page in this.currentPage.Navigation.NavigationStack.Reverse().ToList())
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

        public async Task<string> PushViewModelWithNewNavigation<T>(object data, bool animate = true) where T : BaseViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>();
            var navigationName = Guid.NewGuid().ToString();
            var naviationContainer = new NavigationContainer(page, navigationName);
            await PushNewNavigationServiceModal(naviationContainer, page.GetModel(), animate);
            return navigationName;
        }

        public Task PushNewNavigationServiceModal(TabbedNavigationContainer tabbedNavigationContainer, BaseViewModel baseViewModel = null, bool animate = true)
        {
            var models = tabbedNavigationContainer.TabbedPages.Select(o => o.GetModel()).ToList();
            if (baseViewModel != null)
                models.Add(baseViewModel);
            return PushNewNavigationServiceModal(tabbedNavigationContainer, models.ToArray(), animate);
        }

        public Task PushNewNavigationServiceModal(INavigationService newNavigationService, BaseViewModel basePageModels, bool animate = true)
        {
            return PushNewNavigationServiceModal(newNavigationService, new BaseViewModel[] { basePageModels }, animate);
        }

        public async Task PushNewNavigationServiceModal(INavigationService newNavigationService, BaseViewModel[] basePageModels, bool animate = true)
        {
            if (!(newNavigationService is Page navPage))
                throw new Exception("Navigation service is not Page");

            foreach (var pageModel in basePageModels)
            {
                pageModel.CurrentNavigationServiceName = newNavigationService.NavigationServiceName;
                pageModel.PreviousNavigationServiceName = currentViewModel.CurrentNavigationServiceName;
                pageModel.IsModalFirstChild = true;
            }

            await TinyIoC.TinyIoC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PushPage(navPage, true, animate);
        }

        public Task PushNewNavigationServiceModal(MasterDetailNavigationContainer masterDetailContainer, BaseViewModel basePageModel = null, bool animate = true)
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

        public async Task PopModalNavigationService(bool animate = true)
        {
            var currentNavigationService = TinyIoC.TinyIoC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName);
            currentNavigationService.NotifyChildrenPageWasPopped();

            TinyIoC.TinyIoC.Container.Unregister<INavigationService>(currentViewModel.CurrentNavigationServiceName);

            var navServiceName = currentViewModel.PreviousNavigationServiceName;
            await TinyIoC.TinyIoC.Container.Resolve<INavigationService>(navServiceName).PopPage(animate);
        }

        public void SwitchOutRootNavigation(string navigationServiceName)
        {
            INavigationService rootNavigation = TinyIoC.TinyIoC.Container.Resolve<INavigationService>(navigationServiceName);

            if (!(rootNavigation is Page))
                throw new Exception("Navigation service is not a page");

            Application.Current.MainPage = rootNavigation as Page;
        }

        public Task<BaseViewModel> SwitchSelectedRootPageModel<T>() where T : BaseViewModel
        {
            return TinyIoC.TinyIoC.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).SwitchSelectedRootViewModel<T>();
        }

        public Task<BaseViewModel> SwitchSelectedTab<T>() where T : BaseViewModel
        {
            return SwitchSelectedRootPageModel<T>();
        }

        public Task<BaseViewModel> SwitchSelectedMaster<T>() where T : BaseViewModel
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