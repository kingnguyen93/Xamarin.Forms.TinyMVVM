using System;
using System.Linq.Expressions;

namespace TinyIoC
{
    public interface IRegisterOptions
    {
        IRegisterOptions AsSingleton();

        IRegisterOptions AsMultiInstance();

        IRegisterOptions WithWeakReference();

        IRegisterOptions WithStrongReference();

        IRegisterOptions UsingConstructor<RegisterType>(Expression<Func<RegisterType>> constructor);
    }
}