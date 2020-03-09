using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal static class StoreBuilder
    {
        public static StoreBuilder<TStore> CopyFrom<TStore>(TStore store)
            where TStore : class
        {
            DynamicStateProperty dynamicState = DynamicStateProperty.Unbox(store);
            if (dynamicState == null)
            {
                throw new ArgumentException($"Invalid underlying type of {nameof(store)}", nameof(store));
            }

            ObservableProperty copy = ObservableProperty.CreateCopy(dynamicState.ObservableProperty);

            return new StoreBuilder<TStore>(copy);
        }

        public static StoreBuilder<TState> DefaultFrom<TState>(TState state)
            where TState : class
        {
            DynamicStateProperty dynamicState = DynamicStateProperty.Unbox(state);
            if (dynamicState == null)
            {
                throw new ArgumentException($"Invalid underlying type of {nameof(state)}", nameof(state));
            }

            // TODO: Use default state from service collection registration
            ObservableProperty copy = ObservableProperty.CreateEmptyCopy(dynamicState.ObservableProperty);

            return new StoreBuilder<TState>(copy);
        }
    }

    internal class StoreBuilder<TStore>
        where TStore : class
    {
        private readonly ObservableProperty observableProperty;

        internal StoreBuilder(ObservableProperty observableProperty)
        {
            this.observableProperty = observableProperty;
        }

        public StoreBuilder<TStore> WithValue<TValue>(Expression<Func<TStore, TValue>> expression, TValue value)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;

            string memberName = memberExpression.Member.Name;
            if (!observableProperty.TrySetMember(memberName, value))
            {
                throw new Exception($"Could not set member {memberName}");
            }

            return this;
        }

        public TStore Build()
        {
            var dynamicState = DynamicStateProperty.Create(observableProperty);
            return DynamicStateProperty.Box<TStore>(dynamicState);
        }
    }
}
