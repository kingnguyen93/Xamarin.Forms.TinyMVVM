using System;

namespace TinyMVVM
{
    public static class ViewModelMapper
    {
        public static string GetPageTypeName(Type viewModelType)
        {
            return viewModelType.AssemblyQualifiedName
                .Replace("ViewModel", "Page")
                .Replace("Pages", "Views");
        }

        public static string GetViewModelTypeName(Type viewType)
        {
            return viewType.AssemblyQualifiedName
                .Replace("Views", "ViewModels")
                .Replace("Page", "ViewModel");
        }
    }
}