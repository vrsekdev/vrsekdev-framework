using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.StoreAccessors
{
    internal class ReflectionConsumerWrapper : IConsumerWrapper
    {
        #region static
        private static readonly Func<ComponentBase, Action, Task> ComponentBaseInvokeAsync;
        private static readonly Action<ComponentBase> ComponentBaseStateHasChanged;

        static ReflectionConsumerWrapper()
        {
            MethodInfo invokeAsyncMethodInfo =
            typeof(ComponentBase).GetMethod(
                name: "InvokeAsync",
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(Action) },
                modifiers: null);
            ComponentBaseInvokeAsync = (Func<ComponentBase, Action, Task>)
                Delegate.CreateDelegate(typeof(Func<ComponentBase, Action, Task>), invokeAsyncMethodInfo);

            MethodInfo stateHasChangedMethodInfo =
                typeof(ComponentBase).GetMethod(
                    name: "StateHasChanged",
                    bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

            ComponentBaseStateHasChanged = (Action<ComponentBase>)Delegate.CreateDelegate(typeof(Action<ComponentBase>), stateHasChangedMethodInfo);
        }
        #endregion

        private readonly WeakReference<ComponentBase> consumerReference;

        public ReflectionConsumerWrapper(ComponentBase consumer)
        {
            this.consumerReference = new WeakReference<ComponentBase>(consumer);
        }

        public Task ForceUpdate()
        {
            if (!consumerReference.TryGetTarget(out ComponentBase consumer))
            {
#if DEBUG
                throw new Exception("Component is dead");
#else
                return Task.CompletedTask;
#endif
            }

            return ComponentBaseInvokeAsync(consumer, () => ComponentBaseStateHasChanged(consumer));
        }

        public bool IsRendered()
        {
            return true;
        }

        public bool IsAlive()
        {
            return consumerReference.TryGetTarget(out _);
        }
    }
}
