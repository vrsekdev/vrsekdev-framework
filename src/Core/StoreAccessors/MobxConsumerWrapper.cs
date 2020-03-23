using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.StoreAccessors
{
    internal class MobxConsumerWrapper : IConsumerWrapper
    {
        private readonly WeakReference<IBlazorMobxComponent> consumerReference;
        private readonly string componentName;

        public MobxConsumerWrapper(IBlazorMobxComponent consumer)
        {
            this.consumerReference = new WeakReference<IBlazorMobxComponent>(consumer);
            this.componentName = consumer.GetType().Name;
        }

        public Task ForceUpdate()
        {
            if (!consumerReference.TryGetTarget(out IBlazorMobxComponent consumer))
            {
#if DEBUG
                throw new Exception("Component is dead");
#else
                return Task.CompletedTask;
#endif
            }

            return consumer.ForceUpdate();
        }

        public bool IsAlive()
        {
            return consumerReference.TryGetTarget(out _);
        }

        public bool IsRendered()
        {
            if (!consumerReference.TryGetTarget(out IBlazorMobxComponent consumer))
            {
#if DEBUG
                throw new Exception("Component is dead");
#else
                return Task.CompletedTask;
#endif
            }

            return consumer.IsRendered();
        }

        public override string ToString()
        {
            return componentName;
        }
    }
}
