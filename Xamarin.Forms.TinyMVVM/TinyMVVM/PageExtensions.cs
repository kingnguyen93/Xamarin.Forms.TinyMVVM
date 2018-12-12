using System.Linq;

namespace Xamarin.Forms.TinyMVVM
{
    public static class PageExtensions
    {
        public static TinyViewModel GetModel(this Page page)
        {
            return page.BindingContext as TinyViewModel;
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

        public static bool IsModal(this Page page)
        {
            for (int i = 0; i < page.Navigation.ModalStack.Count(); i++)
            {
                if (page == page.Navigation.ModalStack[i])
                    return true;
            }
            return false;
        }
    }
}