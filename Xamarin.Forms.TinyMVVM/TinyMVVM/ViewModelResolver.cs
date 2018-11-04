using System;

namespace Xamarin.Forms.TinyMVVM
{
    public static class ViewModelResolver
    {
        public static Page ResolveViewModel<T>() where T : TinyViewModel
        {
            T viewModel = TinyIOC.Container.Resolve<T>();
            return ResolveViewModel(viewModel);
        }

        public static Page ResolveViewModel<T>(object data) where T : TinyViewModel
        {
            T viewModel = TinyIOC.Container.Resolve<T>();
            return ResolveViewModel(viewModel, data);
        }

        public static Page ResolveViewModel<T>(NavigationParameters parameters) where T : TinyViewModel
        {
            T viewModel = TinyIOC.Container.Resolve<T>();
            return ResolveViewModel(viewModel, parameters: parameters);
        }

        public static Page ResolveViewModel(Type type)
        {
            var viewModel = TinyIOC.Container.Resolve(type) as TinyViewModel;
            return ResolveViewModel(viewModel);
        }

        public static Page ResolveViewModel(Type type, object data)
        {
            var viewModel = TinyIOC.Container.Resolve(type) as TinyViewModel;
            return ResolveViewModel(viewModel, data);
        }

        public static Page ResolveViewModel(Type type, NavigationParameters parameters)
        {
            var viewModel = TinyIOC.Container.Resolve(type) as TinyViewModel;
            return ResolveViewModel(viewModel, parameters: parameters);
        }

        public static Page ResolveViewModel(TinyViewModel viewModel, object data = null, NavigationParameters parameters = null)
        {
            //TinyIOC.Container.BuildUp(ref viewModel);
            var pageName = ViewModelMapper.GetPageTypeName(viewModel.GetType());
            var pageType = Type.GetType(pageName);
            if (pageType == null)
                throw new Exception(pageName + " not found");

            Page page = (Page)TinyIOC.Container.Resolve(pageType);

            return BindingPageModel(page, viewModel, data, parameters);
        }

        public static Page BindingPageModel(Page targetPage, TinyViewModel viewModel, object data = null, NavigationParameters parameters = null)
        {
            viewModel.CurrentPage = targetPage;
            viewModel.Parameters = parameters;
            viewModel.Init(data);
            viewModel.CoreMethods = new ViewModelCoreMethods(targetPage, viewModel);
            viewModel.WireEvents();
            targetPage.BindingContext = viewModel;
            viewModel.OnPageCreated();
            return targetPage;
        }
    }
}