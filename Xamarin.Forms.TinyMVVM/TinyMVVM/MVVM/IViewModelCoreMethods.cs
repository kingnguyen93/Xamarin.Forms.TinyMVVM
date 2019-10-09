using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TinyMVVM
{
    public interface IViewModelCoreMethods
    {
        #region PageDialogs

        Task DisplayAlert(string title, string message, string cancel);

        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);

        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);

        #endregion PageDialogs

        #region NavigationService

        Task PushPage<TPage>(bool modal = false, bool animate = true) where TPage : Page;

        Task PushPage<TPage>(NavigationParameters parameters, bool modal = false, bool animate = true) where TPage : Page;

        Task PushPage(Type pageType, bool modal = false, bool animate = true);

        Task PushPage(Type pageType, NavigationParameters parameters, bool modal = false, bool animate = true);

        Task PushPage(Page page, bool modal = false, bool animate = true);

        Task PushPage(Page page, NavigationParameters parameters, bool modal = false, bool animate = true);

        Task PushViewModel<TViewModel>(bool modal = false, bool animate = true) where TViewModel : TinyViewModel;

        Task PushViewModel<TViewModel>(NavigationParameters parameters, bool modal = false, bool animate = true) where TViewModel : TinyViewModel;

        Task PushViewModel(Type viewModelType, bool modal = false, bool animate = true);

        Task PushViewModel(Type viewModelType, NavigationParameters parameters, bool modal = false, bool animate = true);

        Task PushViewModel<TViewModel, TPage>(bool modal = false, bool animate = true) where TViewModel : TinyViewModel where TPage : Page;

        Task PushViewModel<TViewModel, TPage>(NavigationParameters parameters, bool modal = false, bool animate = true) where TViewModel : TinyViewModel where TPage : Page;

        Task PushViewModel(Type viewModelType, Type pageType, bool modal = false, bool animate = true);

        Task PushViewModel(Type viewModelType, Type pageType, NavigationParameters parameters, bool modal = false, bool animate = true);

        Task PopViewModel(bool modal = false, bool animate = true);

        Task PopViewModel(NavigationParameters parameters, bool modal = false, bool animate = true);

        Task PopToRoot(bool animate);

        Task PopAllPopup(bool animate);

        /// <summary>
        /// Removes current page/pagemodel from navigation
        /// </summary>
        void RemoveFromNavigation();

        /// <summary>
        /// Removes specific page/pagemodel from navigation
        /// </summary>
        /// <param name="removeAll">Will remove all, otherwise it will just remove first on from the top of the stack</param>
        /// <typeparam name="TPageModel">The 1st type parameter.</typeparam>
        void RemoveFromNavigation<T>(bool removeAll = false) where T : TinyViewModel;

        /// <summary>
        /// This method pushes a new PageModel modally with a new NavigationContainer
        /// </summary>
        /// <returns>Returns the name of the new service</returns>
        Task<string> PushViewModelWithNewNavigation<T>(bool animate = true) where T : TinyViewModel;

        Task<string> PushViewModelWithNewNavigation<T>(NavigationParameters parameters, bool animate = true) where T : TinyViewModel;

        Task<string> PushViewModelWithNewNavigation<TROOT, T>(NavigationParameters parameters = default, bool animate = true) where T : TinyViewModel where TROOT : INavigationService;

        Task PushNewNavigationServiceModal(INavigationService newNavigationService, TinyViewModel basePageModels, NavigationParameters parameters = default, bool animate = true);

        Task PushNewNavigationServiceModal(INavigationService newNavigationService, TinyViewModel[] basePageModels, NavigationParameters parameters = default, bool animate = true);

        Task PushNewNavigationServiceModal(TabbedNavigationContainer tabbedNavigationContainer, TinyViewModel baseViewModel = null, NavigationParameters parameters = default, bool animate = true);

        Task PushNewNavigationServiceModal(MasterDetailNavigationContainer masterDetailContainer, TinyViewModel baseViewModel = null, NavigationParameters parameters = default, bool animate = true);

        Task PopModalNavigationService(NavigationParameters parameters = default, bool animate = true);

        void SwitchOutRootNavigation(string navigationServiceName);

        /// <summary>
        /// This method is used when you want to switch the selected page,
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelectedTab">The pagemodel of the root you want to change</param>
        Task<TinyViewModel> SwitchSelectedTab<T>() where T : TinyViewModel;

        /// <summary>
        /// This method is used when you want to switch the selected page,
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelectedMaster">The pagemodel of the root you want to change</param>
        Task<TinyViewModel> SwitchSelectedMaster<T>() where T : TinyViewModel;

        /// <summary>
        /// This method switches the selected main page, TabbedPage the selected tab or if MasterDetail, works with custom pages also
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelected">The pagemodel of the root you want to change</param>
        Task<TinyViewModel> SwitchSelectedRootPageModel<T>() where T : TinyViewModel;

        void NotifyAllChildrenPopped();

        #endregion NavigationService

        void BatchBegin();

        void BatchCommit();
    }
}