using System;
using System.Reflection;

namespace TinyMVVM.Extensions
{
    public static class PropertyExtension
    {
        public static void SetPropertyValue<T>(this T inputObject, string propertyName, object propertyVal) where T : ExtendedBindableObject
        {
            try
            {
                //find out the type
                Type type = inputObject.GetType();

                //get the property information based on the type
                PropertyInfo propertyInfo = type.GetProperty(propertyName);

                //find the property type
                Type propertyType = propertyInfo.PropertyType;

                //Convert.ChangeType does not handle conversion to nullable types
                //if the property type is nullable, we need to get the underlying type of the property
                var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

                //Returns an System.Object with the specified System.Type and whose value is
                //equivalent to the specified object.
                propertyVal = Convert.ChangeType(propertyVal, targetType);

                //Set the value of the property
                propertyInfo.SetValue(inputObject, propertyVal, null);

                (inputObject as ExtendedBindableObject)?.RaisePropertyChanged(propertyName);
            }
            catch { }
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}