using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TinyMVVM
{
    public abstract class ExtendedBindableObject : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, Action onChanged, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            return true;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", params string[] relatedProperties)
        {
            return SetProperty(ref backingStore, value, null, propertyName, relatedProperties);
        }

        protected bool SetProperty<T>(ref T backingStore, T value, Action onChanged, [CallerMemberName] string propertyName = "", params string[] relatedProperties)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            foreach (var property in relatedProperties)
            {
                OnPropertyChanged(property);
            }

            return true;
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }

        public void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            OnPropertyChanged(GetMemberInfo(property).Name);
        }

        private MemberInfo GetMemberInfo(Expression expression)
        {
            MemberExpression operand;
            LambdaExpression lambdaExpression = (LambdaExpression)expression;
            if (lambdaExpression.Body as UnaryExpression != null)
            {
                UnaryExpression body = (UnaryExpression)lambdaExpression.Body;
                operand = (MemberExpression)body.Operand;
            }
            else
            {
                operand = (MemberExpression)lambdaExpression.Body;
            }
            return operand.Member;
        }

        public void RaisePropertyChanged<TProperty>(params Expression<Func<TProperty>>[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                OnPropertyChanged(GetPropertyInfo(properties[i]).Name);
            }
        }

        private PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TProperty>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (!(property.Body is MemberExpression body))
                throw new ArgumentException("Expression is not a property", nameof(property));

            var propertyInfo = body.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Expression is not a property", nameof(property));

            return propertyInfo;
        }

        public void OnPropertyChanged(params string[] properties)
        {
            foreach (var property in properties)
            {
                OnPropertyChanged(property);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}