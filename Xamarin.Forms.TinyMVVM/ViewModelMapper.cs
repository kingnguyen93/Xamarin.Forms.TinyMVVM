using System;

namespace TinyMVVM
{
    public class ViewModelMapper
    {
        public static string GetPageTypeName(Type viewModelType)
        {
            return viewModelType.AssemblyQualifiedName
                .Replace("ViewModel", "View");
        }

        public static string GetViewModelTypeName(Type viewType)
        {
            return viewType.AssemblyQualifiedName
                .Replace("View", "ViewModel");
        }
    }
}