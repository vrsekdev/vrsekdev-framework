using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    public class ActionBuilder
    {
        public static ActionBuilder<TStore> For<TStore>(Action<TStore> action)
        {
            return new ActionBuilder<TStore>(action);
        }
    }

    public class ActionBuilderFactory<TStore>
    {
        public ActionBuilder<TStore> For(Action<TStore> action)
        {
            return new ActionBuilder<TStore>(action);
        }
    }

    public class ActionBuilder<TStore>
    {
        internal Action<TStore> Action { get; }

        internal HashSet<PropertyInfo> ObservedProperties { get; } = new HashSet<PropertyInfo>();
        internal HashSet<PropertyInfo> ObservedCollections { get; } = new HashSet<PropertyInfo>();


        public static ActionBuilder<TStore> For(Action<TStore> action)
        {
            return new ActionBuilder<TStore>(action);
        }

        internal ActionBuilder(Action<TStore> action)
        {
            Action = action;
        }

        public ActionBuilder<TStore> Observe<T>(Expression<Func<TStore, T>> propertyExpression)
        {
            ObservedProperties.Add(GetPropertyInfo(propertyExpression));

            return this;
        }

        public ActionBuilder<TStore> Observe<T>(Expression<Func<TStore, IObservableCollection<T>>> collectionExpression)
        {
            ObservedCollections.Add(GetPropertyInfo(collectionExpression));

            return this;
        }

        private PropertyInfo GetPropertyInfo(LambdaExpression expression)
        {
            MemberExpression member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(String.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    expression.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(String.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    expression.ToString()));

            return propInfo;
        }
    }
}
