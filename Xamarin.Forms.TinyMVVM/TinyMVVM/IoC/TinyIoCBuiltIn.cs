using System;
using TinyIoC;

namespace Xamarin.Forms.TinyMVVM
{
    /// <summary>
    /// Built in TinyIOC for ease of use
    /// </summary>
    public class TinyIOCBuiltIn : ITinyIOC
    {
        public static TinyIoCContainer Current
        {
            get
            {
                return TinyIoCContainer.Current;
            }
        }

        public IRegisterOptions Register<RegisterType>(RegisterType instance, string name) where RegisterType : class
        {
            return TinyIoCContainer.Current.Register(instance, name);
        }

        public IRegisterOptions Register<RegisterType>(RegisterType instance) where RegisterType : class
        {
            return TinyIoCContainer.Current.Register(instance);
        }

        public ResolveType Resolve<ResolveType>(string name) where ResolveType : class
        {
            var result = TinyIoCContainer.Current.Resolve<ResolveType>(name);
            BuildUp(ref result);
            return result;
        }

        public ResolveType Resolve<ResolveType>() where ResolveType : class
        {
            var result = TinyIoCContainer.Current.Resolve<ResolveType>();
            BuildUp(ref result);
            return result;
        }

        public IRegisterOptions Register<RegisterType, RegisterImplementation>()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return TinyIoCContainer.Current.Register<RegisterType, RegisterImplementation>();
        }

        public object Resolve(Type resolveType)
        {
            var result = TinyIoCContainer.Current.Resolve(resolveType);
            BuildUp(ref result);
            return result;
        }

        public void BuildUp<ResolveType>(ref ResolveType input) where ResolveType : class
        {
            TinyIoCContainer.Current.BuildUp(input);
        }

        public void Unregister<RegisterType>()
        {
            TinyIoCContainer.Current.Unregister<RegisterType>();
        }

        public void Unregister<RegisterType>(string name)
        {
            TinyIoCContainer.Current.Unregister<RegisterType>(name);
        }
    }
}