using Xamarin.Forms;

namespace TinyMVVM
{
    public static class PageExtensions
    {
        public static BaseViewModel GetModel(this Page page)
        {
            return page.BindingContext as BaseViewModel;
        }

        public static void NotifyAllChildrenPopped(this NavigationPage navigationPage)
        {
            foreach (var page in navigationPage.Navigation.ModalStack)
            {
                var viewModel = page.GetModel();
                if (viewModel != null)
                    viewModel.RaisePageWasPopped();
            }

            foreach (var page in navigationPage.Navigation.NavigationStack)
            {
                var viewModel = page.GetModel();
                if (viewModel != null)
                    viewModel.RaisePageWasPopped();
            }
        }
    }
}