using System;

namespace Xamarin.Forms.TinyMVVM
{
    public static class ViewModelResolver
    {
        public static Page ResolveViewModel<T>() where T : TinyViewModel
        {
            return ResolveViewModel<T>(null);
        }

        public static Page ResolveViewModel<T>(object data) where T : TinyViewModel
        {
            var viewModel = TinyIOC.Container.Resolve<T>();
            return ResolveViewModel(viewModel, data);
        }

        public static Page ResolveViewModel<T>(T viewModel, object data) where T : TinyViewModel
        {
            return ResolveViewModel(viewModel.GetType(), data);
        }

        public static Page ResolveViewModel(Type type)
        {
            var viewModel = TinyIOC.Container.Resolve(type) as TinyViewModel;
            return ResolveViewModel(viewModel, null);
        }

        public static Page ResolveViewModel(Type type, object data)
        {
            var viewModel = TinyIOC.Container.Resolve(type) as TinyViewModel;
            return ResolveViewModel(viewModel, data);
        }

        public static Page ResolveViewModel(TinyViewModel viewModel, object data)
        {
            var pageName = ViewModelMapper.GetPageTypeName(viewModel.GetType());
            var pageType = Type.GetType(pageName);
            if (pageType == null)
                throw new Exception(pageName + " not found");

            var page = (Page)TinyIOC.Container.Resolve(pageType);

            return BindingPageModel(page, viewModel, data);
        }

        public static Page BindingPageModel(Page targetPage, TinyViewModel viewModel, object data)
        {
            viewModel.CurrentPage = targetPage;
            viewModel.WireEvents();
            viewModel.CoreMethods = new ViewModelCoreMethods(targetPage, viewModel);
            viewModel.Init(data);
            targetPage.BindingContext = viewModel;
            return targetPage;
        }
    }
}