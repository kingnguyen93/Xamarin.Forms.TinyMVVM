using System;

namespace Xamarin.Forms.TinyMVVM
{
    public class ViewModelMapper
    {
        public static string GetPageTypeName(Type viewModelType)
        {
            return viewModelType.AssemblyQualifiedName
                .Replace("ViewModel", "Page");
        }

        public static string GetViewModelTypeName(Type viewType)
        {
            return viewType.AssemblyQualifiedName
                .Replace("Page", "ViewModel");
        }
    }
}