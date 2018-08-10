using System;

namespace Xamarin.Forms
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
                .Replace("View", "ViewModel")
                .Replace("ViewModel", "");
        }
    }
}