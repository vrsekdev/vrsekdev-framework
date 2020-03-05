using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    public static class StateBuilder
    {
        public static StateBuilder<TState> CopyFrom<TState>(TState state)
            where TState : class
        {
            DynamicStateProperty dynamicState = DynamicStateProperty.Unbox(state);
            if (dynamicState == null)
            {
                throw new ArgumentException($"Invalid underlying type of {nameof(state)}", nameof(state));
            }

            ObservableProperty copy = ObservableProperty.CreateCopy(dynamicState.ObservableProperty);

            return new StateBuilder<TState>(copy);
        }

        public static StateBuilder<TState> DefaultFrom<TState>(TState state)
            where TState : class
        {
            DynamicStateProperty dynamicState = DynamicStateProperty.Unbox(state);
            if (dynamicState == null)
            {
                throw new ArgumentException($"Invalid underlying type of {nameof(state)}", nameof(state));
            }

            ObservableProperty copy = ObservableProperty.CreateEmptyCopy(dynamicState.ObservableProperty);

            return new StateBuilder<TState>(copy);
        }
    }

    public class StateBuilder<TState>
        where TState : class
    {
        private readonly ObservableProperty observableProperty;

        internal StateBuilder(ObservableProperty observableProperty)
        {
            this.observableProperty = observableProperty;
        }

        public StateBuilder<TState> WithValue<TValue>(Expression<Func<TState, TValue>> expression, TValue value)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;

            string memberName = memberExpression.Member.Name;
            if (!observableProperty.TrySetMember(memberName, value))
            {
                throw new Exception($"Could not set member {memberName}");
            }

            return this;
        }

        public TState Build()
        {
            var dynamicState = DynamicStateProperty.Create(observableProperty);
            return DynamicStateProperty.Box<TState>(dynamicState);
        }
    }
}
