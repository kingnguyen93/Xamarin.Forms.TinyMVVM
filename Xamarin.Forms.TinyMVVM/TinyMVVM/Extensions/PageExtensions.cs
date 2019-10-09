using System.Linq;

using Xamarin.Forms;

namespace TinyMVVM.Extensions
{
    public static class PageExtension
    {
        public static TinyViewModel GetModel(this Page page)
        {
            return page.BindingContext as TinyViewModel;
        }

        public static bool TryGetModel(this Page page, out TinyViewModel viewModel)
        {
            viewModel = page.BindingContext as TinyViewModel;

            return viewModel != null;
        }

        public static void NotifyAllChildrenPopped(this NavigationPage navigationPage)
        {
            foreach (var page in navigationPage.Navigation.NavigationStack)
            {
                var viewModel = page.GetModel();
                if (viewModel != null)
                    viewModel.RaisePageWasPopped();
            }

            foreach (var page in navigationPage.Navigation.ModalStack)
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

        public static NavigationPage OnNavigatedTo(this NavigationPage navigationPage, NavigationParameters parameters)
        {
            if (navigationPage.RootPage.BindingContext is TinyViewModel viewModel)
            {
                viewModel.OnNavigatedToAsync(parameters);
                viewModel.OnNavigatedTo(parameters);
            }

            return navigationPage;
        }
    }
}