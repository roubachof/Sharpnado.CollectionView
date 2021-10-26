using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Sharpnado.CollectionView.ViewModels
{
    public class Bindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("Getting property name form expression is not supported for this type.");
            }

            if (!(expression is LambdaExpression lamda))
            {
                throw new NotSupportedException("Getting property name form expression is not supported for this type.");
            }

            switch (lamda.Body)
            {
                case MemberExpression memberExpression:
                    RaisePropertyChanged(memberExpression.Member.Name);
                    return;
                case UnaryExpression unary when unary.Operand is MemberExpression member:
                    RaisePropertyChanged(member.Member.Name);
                    return;
                default:
                    throw new NotSupportedException("Getting property name form expression is not supported for this type.");
            }
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetAndRaise<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, value))
            {
                return false;
            }

            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
