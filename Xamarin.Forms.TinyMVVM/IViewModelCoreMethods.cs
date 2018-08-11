using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TinyMVVM
{
    public interface IViewModelCoreMethods
    {
        Task DisplayAlert(string title, string message, string cancel);

        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);

        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);

        Task PushPage<T>(bool modal = false, bool animate = true) where T : Page;

        Task PushPage(Type pageType, bool modal = false, bool animate = true);

        Task PushPage(Page page, bool modal = false, bool animate = true);

        Task PushViewModel<T>(object data = null, bool modal = false, bool animate = true) where T : BaseViewModel;

        Task PushViewModel<T>(NavigationParameters parameters, bool modal = false, bool animate = true) where T : BaseViewModel;

        Task PushViewModel<T, TPage>(object data = null, bool modal = false, bool animate = true) where T : BaseViewModel where TPage : Page;

        Task PushViewModel<T, TPage>(NavigationParameters parameters, bool modal = false, bool animate = true) where T : BaseViewModel where TPage : Page;

        Task PushViewModel(Type viewModelType, object data = null, bool modal = false, bool animate = true);

        Task PushViewModel(Type viewModelType, NavigationParameters parameters, bool modal = false, bool animate = true);

        Task PushViewModel(Type viewModelType, Type pageType, object data = null, bool modal = false, bool animate = true);

        Task PushViewModel(Type viewModelType, Type pageType, NavigationParameters parameters, bool modal = false, bool animate = true);

        Task PopViewModel(bool modal = false, bool animate = true);

        /// <summary>
        /// Removes current page/pagemodel from navigation
        /// </summary>
        void RemoveFromNavigation();

        /// <summary>
        /// Removes specific page/pagemodel from navigation
        /// </summary>
        /// <param name="removeAll">Will remove all, otherwise it will just remove first on from the top of the stack</param>
        /// <typeparam name="TPageModel">The 1st type parameter.</typeparam>
        void RemoveFromNavigation<T>(bool removeAll = false) where T : BaseViewModel;

        /// <summary>
        /// This method pushes a new PageModel modally with a new NavigationContainer
        /// </summary>
        /// <returns>Returns the name of the new service</returns>
        Task<string> PushViewModelWithNewNavigation<T>(object data, bool animate = true) where T : BaseViewModel;

        Task PushNewNavigationServiceModal(INavigationService newNavigationService, BaseViewModel basePageModels, bool animate = true);

        Task PushNewNavigationServiceModal(INavigationService newNavigationService, BaseViewModel[] basePageModels, bool animate = true);

        Task PushNewNavigationServiceModal(TabbedNavigationContainer tabbedNavigationContainer, BaseViewModel baseViewModel = null, bool animate = true);

        Task PushNewNavigationServiceModal(MasterDetailNavigationContainer masterDetailContainer, BaseViewModel baseViewModel = null, bool animate = true);

        Task PopModalNavigationService(bool animate = true);

        void SwitchOutRootNavigation(string navigationServiceName);

        /// <summary>
        /// This method switches the selected main page, TabbedPage the selected tab or if MasterDetail, works with custom pages also
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelected">The pagemodel of the root you want to change</param>
        Task<BaseViewModel> SwitchSelectedRootPageModel<T>() where T : BaseViewModel;

        /// <summary>
        /// This method is used when you want to switch the selected page,
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelectedTab">The pagemodel of the root you want to change</param>
        Task<BaseViewModel> SwitchSelectedTab<T>() where T : BaseViewModel;

        /// <summary>
        /// This method is used when you want to switch the selected page,
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelectedMaster">The pagemodel of the root you want to change</param>
        Task<BaseViewModel> SwitchSelectedMaster<T>() where T : BaseViewModel;

        Task PopToRoot(bool animate);

        void BatchBegin();

        void BatchCommit();
    }
}