using System;
using Xamarin.Forms.IoC;

namespace Xamarin.Forms
{
    public static class ViewModelResolver
    {
        public static Page ResolveViewModel<T>() where T : BaseViewModel
        {
            return ResolveViewModel<T>(null);
        }

        public static Page ResolveViewModel<T>(object data) where T : BaseViewModel
        {
            var viewModel = FormsIoC.Container.Resolve<T>();
            return ResolveViewModel(viewModel, data);
        }

        public static Page ResolveViewModel<T>(T viewModel, object data) where T : BaseViewModel
        {
            return ResolveViewModel(viewModel.GetType(), data);
        }

        public static Page ResolveViewModel(Type type)
        {
            var viewModel = FormsIoC.Container.Resolve(type) as BaseViewModel;
            return ResolveViewModel(viewModel, null);
        }

        public static Page ResolveViewModel(Type type, object data)
        {
            var viewModel = FormsIoC.Container.Resolve(type) as BaseViewModel;
            return ResolveViewModel(viewModel, data);
        }

        public static Page ResolveViewModel(BaseViewModel viewModel, object data)
        {
            var pageName = ViewModelMapper.GetPageTypeName(viewModel.GetType());
            var pageType = Type.GetType(pageName);
            if (pageType == null)
                throw new Exception(pageName + " not found");

            var page = (Page)FormsIoC.Container.Resolve(pageType);

            return BindingPageModel(page, viewModel, data);
        }

        public static Page BindingPageModel(Page targetPage, BaseViewModel viewModel, object data)
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