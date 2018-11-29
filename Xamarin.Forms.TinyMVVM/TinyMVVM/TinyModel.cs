using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.TinyMVVM
{
    public class TinyModel : INotifyPropertyChanged
    {
        private bool isBusy;

        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged<TProperty>(params Expression<Func<TProperty>>[] properties)
        {
            Dictionary<string, Func<TProperty>> propertyNames = new Dictionary<string, Func<TProperty>>();

            for (int i = 0; i < properties.Length; i++)
            {
                if (GetPropertyInfo(properties[i]) is PropertyInfo propertyInfo)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyInfo.Name));
            }
        }

        public PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TProperty>> property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (!(property.Body is MemberExpression body))
                throw new ArgumentException("Expression is not a property", "property");

            var propertyInfo = body.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Expression is not a property", "property");

            return propertyInfo;
        }
    }
}