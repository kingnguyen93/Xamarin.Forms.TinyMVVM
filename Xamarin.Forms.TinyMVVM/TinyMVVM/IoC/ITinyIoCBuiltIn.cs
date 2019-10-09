using System;
using TinyIoC;

namespace TinyMVVM.IoC
{
    public interface ITinyIoCBuiltIn
    {
        object Resolve(Type resolveType);

        IRegisterOptions Register<RegisterType>(RegisterType instance) where RegisterType : class;

        IRegisterOptions Register<RegisterType>(RegisterType instance, string name) where RegisterType : class;

        ITinyIoCBuiltIn Register(Type from, Type to, string name);

        ResolveType Resolve<ResolveType>() where ResolveType : class;

        ResolveType Resolve<ResolveType>(string name) where ResolveType : class;

        IRegisterOptions Register<RegisterType, RegisterImplementation>()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;

        void BuildUp<ResolveType>(ref ResolveType input) where ResolveType : class;

        void Unregister<RegisterType>();

        void Unregister<RegisterType>(string name);
    }
}