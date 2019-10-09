using System.Threading.Tasks;

using Xamarin.Forms;

namespace TinyMVVM
{
    public interface INavigationService
    {
        string NavigationServiceName { get; }

        Task PushPage(Page page, bool modal = false, bool animate = true);

        Task PopPage(bool modal = false, bool animate = true);

        Task PopToRoot(bool animate = true);

        /// <summary>
        /// This method switches the selected main page, TabbedPage the selected tab or if MasterDetail, works with custom pages also
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelected">The pagemodel of the root you want to change</param>
        Task<TinyViewModel> SwitchSelectedRootViewModel<T>() where T : TinyViewModel;

        void NotifyChildrenPageWasPopped();
    }
}