using System;

namespace Xamarin.Forms.TinyMVVM
{
    public static class ViewModelResolver
    {
        public static Page ResolveViewModel<T>(object data = null, NavigationParameters parameters = null) where T : TinyViewModel
        {
            var viewModel = TinyIOC.Container.Resolve<T>();
            return ResolveViewModel(viewModel, data, parameters);
        }

        public static Page ResolveViewModel<T>(T viewModel, object data = null, NavigationParameters parameters = null) where T : TinyViewModel
        {
            return ResolveViewModel(viewModel.GetType(), data, parameters);
        }

        public static Page ResolveViewModel(Type type, object data = null, NavigationParameters parameters = null)
        {
            var viewModel = TinyIOC.Container.Resolve(type) as TinyViewModel;
            return ResolveViewModel(viewModel, data, parameters);
        }

        public static Page ResolveViewModel(TinyViewModel viewModel, object data = null, NavigationParameters parameters = null)
        {
            var pageName = ViewModelMapper.GetPageTypeName(viewModel.GetType());
            var pageType = Type.GetType(pageName);
            if (pageType == null)
                throw new Exception(pageName + " not found");

            var page = (Page)TinyIOC.Container.Resolve(pageType);

            return BindingPageModel(page, viewModel, data, parameters);
        }

        public static Page BindingPageModel(Page targetPage, TinyViewModel viewModel, object data = null, NavigationParameters parameters = null)
        {
            viewModel.CurrentPage = targetPage;
            viewModel.Init(data);
            viewModel.Parameters = parameters;
            viewModel.CoreMethods = new ViewModelCoreMethods(targetPage, viewModel);
            viewModel.WireEvents();
            targetPage.BindingContext = viewModel;
            viewModel.OnPageCreated();
            return targetPage;
        }
    }
}