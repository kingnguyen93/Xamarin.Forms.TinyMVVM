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
    }
}