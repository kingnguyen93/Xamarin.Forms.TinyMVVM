using System;
using System.Collections.Generic;
using System.Text;
using TinyMVVM.IoC;
using Xamarin.Forms;

namespace TinyMVVM.IoC
{
     public static class ITinyIoCBuiltInExtension
    {
        public static void RegisterForNavigation<TViewModel, TView>(this ITinyIoCBuiltIn container, string name = null) where TViewModel : class where TView : Page
        {
            container.RegisterForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        private static void RegisterForNavigationWithViewModel<TViewModel>(this ITinyIoCBuiltIn container, Type viewType, string name = null) where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            container.RegisterForNavigation(viewType, name);
        }

        public static void RegisterForNavigation(this ITinyIoCBuiltIn container, Type viewType, string name)
        {
            PageNavigationRegistry.Register(name, viewType);

            container.Register(typeof(object), viewType, name);
        }
    }
}
