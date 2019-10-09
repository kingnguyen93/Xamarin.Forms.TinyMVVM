using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using TinyMVVM.Extensions;
using TinyMVVM.IoC;
using Xamarin.Forms;

namespace TinyMVVM
{
    public class ViewModelCoreMethods : IViewModelCoreMethods
    {
        private readonly Page currentPage;
        private readonly TinyViewModel currentViewModel;

        public ViewModelCoreMethods(Page currentPage, TinyViewModel currentViewModel)
        {
            this.currentPage = currentPage;
            this.currentViewModel = currentViewModel;
        }

        #region PageDialogs

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

        #endregion PageDialogs

        #region NavigationService

        public Task PushPage<TPage>(bool modal = false, bool animate = true) where TPage : Page
        {
            return PushPage(typeof(TPage), modal, animate);
        }

        public Task PushPage<TPage>(NavigationParameters parameters, bool modal = false, bool animate = true) where TPage : Page
        {
            return PushPage(typeof(TPage), parameters, modal, animate);
        }

        public Task PushPage(Type pageType, bool modal = false, bool animate = true)
        {
            var page = TinyIoCLocator.Container.Resolve(pageType) as Page;
            return PushPage(page, new NavigationParameters(), modal, animate);
        }

        public Task PushPage(Type pageType, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            var page = TinyIoCLocator.Container.Resolve(pageType) as Page;
            return PushPage(page, parameters, modal, animate);
        }

        public Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            return PushPage(page, new NavigationParameters(), modal, animate);
        }

        public async Task PushPage(Page page, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            await TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PushPage(page, modal, animate);

            if (page.GetModel() is TinyViewModel viewModel)
            {
                _ = viewModel.OnNavigatedToAsync(parameters);
                viewModel.OnNavigatedTo(parameters);
            }

            if (page is INavigatedAware aware)
            {
                _ = aware.OnNavigatedToAsync(parameters);
                aware.OnNavigatedTo(parameters);
            }
        }

        public Task PushViewModel<TViewModel>(bool modal = false, bool animate = true) where TViewModel : TinyViewModel
        {
            return PushViewModel(typeof(TViewModel), modal, animate);
        }

        public Task PushViewModel<TViewModel>(NavigationParameters parameters, bool modal = false, bool animate = true) where TViewModel : TinyViewModel
        {
            return PushViewModel(typeof(TViewModel), parameters, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, bool modal = false, bool animate = true)
        {
            var page = ViewModelResolver.ResolveViewModel(viewModelType);
            return PushViewModelWithPage(page, page.GetModel(), new NavigationParameters(), modal, animate);
        }

        public Task PushViewModel(Type viewModelType, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            var page = ViewModelResolver.ResolveViewModel(viewModelType, parameters);
            return PushViewModelWithPage(page, page.GetModel(), parameters, modal, animate);
        }

        public Task PushViewModel<TViewModel, TPage>(bool modal = false, bool animate = true) where TViewModel : TinyViewModel where TPage : Page
        {
            return PushViewModel(typeof(TViewModel), typeof(TPage), modal, animate);
        }

        public Task PushViewModel<TViewModel, TPage>(NavigationParameters parameters, bool modal = false, bool animate = true) where TViewModel : TinyViewModel where TPage : Page
        {
            return PushViewModel(typeof(TViewModel), typeof(TPage), parameters, modal, animate);
        }

        public Task PushViewModel(Type viewModelType, Type pageType, bool modal = false, bool animate = true)
        {
            var page = ViewModelResolver.ResolveViewModel(viewModelType, pageType);
            return PushViewModelWithPage(page, page.GetModel(), new NavigationParameters(), modal, animate);
        }

        public Task PushViewModel(Type viewModelType, Type pageType, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            var page = ViewModelResolver.ResolveViewModel(viewModelType, pageType, parameters);
            return PushViewModelWithPage(page, page.GetModel(), parameters, modal, animate);
        }

        private async Task PushViewModelWithPage(Page page, TinyViewModel viewModel, NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            viewModel.PreviousViewModel = currentViewModel; //This is the previous page model because it's push to a new one, and this is current
            viewModel.CurrentNavigationServiceName = currentViewModel.CurrentNavigationServiceName;

            if (string.IsNullOrWhiteSpace(viewModel.PreviousNavigationServiceName))
                viewModel.PreviousNavigationServiceName = currentViewModel.PreviousNavigationServiceName;

            viewModel.IsModal = modal;

            if (page is PopupPage)
                await PopupNavigation.Instance.PushAsync((PopupPage)page, animate);
            else
                await TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PushPage(page, modal, animate);

            _ = viewModel.OnNavigatedToAsync(parameters);
            viewModel.OnNavigatedTo(parameters);
            if (page is INavigatedAware aware)
            {
                _ = aware.OnNavigatedToAsync(parameters);
                aware.OnNavigatedTo(parameters);
            }
        }

        public Task PopViewModel(bool modal = false, bool animate = true)
        {
            return PopViewModel(new NavigationParameters(), modal, animate);
        }

        public async Task PopViewModel(NavigationParameters parameters, bool modal = false, bool animate = true)
        {
            if (currentPage is PopupPage)
            {
                await PopupNavigation.Instance.PopAsync(animate);

                currentViewModel.RaisePageWasPopped();

                currentViewModel.PreviousViewModel?.OnNavigatedBack(parameters);
                if (currentViewModel.PreviousViewModel?.CurrentPage is INavigatedAware)
                    ((INavigatedAware)currentViewModel.PreviousViewModel.CurrentPage).OnNavigatedBack(parameters);
            }
            else
            {
                if (currentViewModel.IsModalFirstChild)
                {
                    await PopModalNavigationService(parameters, animate);
                }
                else if (currentViewModel.IsModal || modal)
                {
                    await TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PopPage(true, animate);

                    currentViewModel.RaisePageWasPopped();

                    currentViewModel.PreviousViewModel?.OnNavigatedBack(parameters);
                    if (currentViewModel.PreviousViewModel?.CurrentPage is INavigatedAware)
                        ((INavigatedAware)currentViewModel.PreviousViewModel.CurrentPage).OnNavigatedBack(parameters);
                }
                else
                {
                    await TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PopPage(modal, animate);

                    currentViewModel.PreviousViewModel?.OnNavigatedBack(parameters);
                    if (currentViewModel.PreviousViewModel?.CurrentPage is INavigatedAware)
                        ((INavigatedAware)currentViewModel.PreviousViewModel.CurrentPage).OnNavigatedBack(parameters);
                }
            }
        }

        public async Task PopToRoot(bool animate)
        {
            await TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PopToRoot(animate);
        }

        public async Task PopAllPopup(bool animate)
        {
            await PopupNavigation.Instance.PopAllAsync(animate);
        }

        public void RemoveFromNavigation()
        {
            currentPage.Navigation.RemovePage(currentPage);
            currentViewModel.RaisePageWasPopped();
        }

        public void RemoveFromNavigation<T>(bool removeAll = false) where T : TinyViewModel
        {
            //var pages = currentPage.Navigation.NavigationStack.Where (o => o is T);
            foreach (var page in currentPage.Navigation.NavigationStack.Reverse().ToList())
            {
                if (page.BindingContext is T)
                {
                    currentPage.Navigation.RemovePage(page);
                    page.GetModel()?.RaisePageWasPopped();
                    if (!removeAll)
                        break;
                }
            }
        }

        public async Task<string> PushViewModelWithNewNavigation<T>(bool animate = true) where T : TinyViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>();
            var navigationName = Guid.NewGuid().ToString();
            var naviationContainer = new NavigationContainer(page, navigationName);
            await PushNewNavigationServiceModal(naviationContainer, page.GetModel(), default, animate);
            return navigationName;
        }

        public async Task<string> PushViewModelWithNewNavigation<T>(NavigationParameters parameters, bool animate = true) where T : TinyViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>(parameters);
            var navigationName = Guid.NewGuid().ToString();
            var naviationContainer = new NavigationContainer(page, navigationName, parameters);
            await PushNewNavigationServiceModal(naviationContainer, page.GetModel(), parameters, animate);
            return navigationName;
        }

        public async Task<string> PushViewModelWithNewNavigation<TROOT, T>(NavigationParameters parameters = default, bool animate = true) where T : TinyViewModel where TROOT : INavigationService
        {
            var page = ViewModelResolver.ResolveViewModel<T>(parameters);
            var navigationName = Guid.NewGuid().ToString();
            var naviationContainer = Activator.CreateInstance(typeof(TROOT), page, navigationName, parameters) as INavigationService;
            await PushNewNavigationServiceModal(naviationContainer, page.GetModel(), parameters, animate);
            return navigationName;
        }

        public Task PushNewNavigationServiceModal(INavigationService newNavigationService, TinyViewModel basePageModels, NavigationParameters parameters = default, bool animate = true)
        {
            return PushNewNavigationServiceModal(newNavigationService, new TinyViewModel[] { basePageModels }, parameters, animate);
        }

        public Task PushNewNavigationServiceModal(TabbedNavigationContainer tabbedNavigationContainer, TinyViewModel baseViewModel = null, NavigationParameters parameters = default, bool animate = true)
        {
            var models = tabbedNavigationContainer.TabbedPages.Select(o => o.GetModel()).ToList();

            if (baseViewModel != null)
                models.Add(baseViewModel);

            return PushNewNavigationServiceModal(tabbedNavigationContainer, models.ToArray(), parameters, animate);
        }

        public Task PushNewNavigationServiceModal(MasterDetailNavigationContainer masterDetailContainer, TinyViewModel basePageModel = null, NavigationParameters parameters = default, bool animate = true)
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

            return PushNewNavigationServiceModal(masterDetailContainer, models.ToArray(), parameters, animate);
        }

        public async Task PushNewNavigationServiceModal(INavigationService newNavigationService, TinyViewModel[] baseViewModels, NavigationParameters parameters = default, bool animate = true)
        {
            if (!(newNavigationService is Page navPage))
                throw new Exception("Navigation service is not Page");

            foreach (var viewModel in baseViewModels)
            {
                viewModel.PreviousViewModel = currentViewModel;
                viewModel.CurrentNavigationServiceName = newNavigationService.NavigationServiceName;
                viewModel.PreviousNavigationServiceName = currentViewModel.CurrentNavigationServiceName;
                viewModel.IsModal = true;
                viewModel.IsModalFirstChild = true;
            }

            await TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).PushPage(navPage, true, animate);

            if (!newNavigationService.GetType().Equals(typeof(NavigationContainer)))
            {
                foreach (var viewModel in baseViewModels)
                {
                    _ = viewModel.OnNavigatedToAsync(parameters);
                    viewModel.OnNavigatedTo(parameters);

                    if (viewModel.CurrentPage is INavigatedAware aware)
                    {
                        _ = aware.OnNavigatedToAsync(parameters);
                        aware.OnNavigatedTo(parameters);
                    }
                }
            }
        }

        public async Task PopModalNavigationService(NavigationParameters parameters = default, bool animate = true)
        {
            if (!currentViewModel.IsModalAndHasPreviousNavigationStack || currentPage.Navigation.ModalStack.Count == 0)
                return;

            var currentNavigationService = TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName);

            TinyIoCLocator.Container.Unregister<INavigationService>(currentViewModel.CurrentNavigationServiceName);

            await TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.PreviousNavigationServiceName).PopPage(true, animate);

            currentNavigationService.NotifyChildrenPageWasPopped();

            currentViewModel.PreviousViewModel?.OnNavigatedBack(parameters);
            if (currentViewModel.PreviousViewModel?.CurrentPage is INavigatedAware)
                ((INavigatedAware)currentViewModel.PreviousViewModel.CurrentPage).OnNavigatedBack(parameters);
        }

        public void SwitchOutRootNavigation(string navigationServiceName)
        {
            INavigationService rootNavigation = TinyIoCLocator.Container.Resolve<INavigationService>(navigationServiceName);

            if (!(rootNavigation is Page))
                throw new Exception("Navigation service is not a page");

            Application.Current.MainPage = rootNavigation as Page;
        }

        public Task<TinyViewModel> SwitchSelectedTab<T>() where T : TinyViewModel
        {
            return SwitchSelectedRootPageModel<T>();
        }

        public Task<TinyViewModel> SwitchSelectedMaster<T>() where T : TinyViewModel
        {
            return SwitchSelectedRootPageModel<T>();
        }

        public Task<TinyViewModel> SwitchSelectedRootPageModel<T>() where T : TinyViewModel
        {
            return TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).SwitchSelectedRootViewModel<T>();
        }

        public void NotifyAllChildrenPopped()
        {
            TinyIoCLocator.Container.Resolve<INavigationService>(currentViewModel.CurrentNavigationServiceName).NotifyChildrenPageWasPopped();
        }

        #endregion NavigationService

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