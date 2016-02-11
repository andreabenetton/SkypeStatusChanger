using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

namespace SkypeStatusChanger.ViewModels
{
    public abstract  class BaseViewModel : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpresion)
        {
            var property = (MemberExpression)propertyExpresion.Body;
            VerifyPropertyExpression<T>(propertyExpresion, property);
            OnPropertyChanged(property.Member.Name);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T refValue, T newValue, Expression<Func<T>> propertyExpresion)
        {
            if (!Equals(refValue, newValue))
            {
                refValue = newValue;
                OnPropertyChanged(propertyExpresion);
            }
        }

        protected void SetValue<T>(ref T refValue, T newValue, Action valueChanged)
        {
            if (!Equals(refValue, newValue))
            {
                refValue = newValue;
                valueChanged();
            }
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyExpression<T>(
    Expression<Func<T>> propertyExpresion,
    MemberExpression property)
        {
            if (property.Member.GetType().IsAssignableFrom(typeof(PropertyInfo)))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Invalid Property Expression {0}",
                    propertyExpresion));
            }

            var instance = property.Expression as ConstantExpression;
            if (instance.Value != this)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Invalid Property Expression {0}",
                    propertyExpresion));
            }
        }

    }
}
