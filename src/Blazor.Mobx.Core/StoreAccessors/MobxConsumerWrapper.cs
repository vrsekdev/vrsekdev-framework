using VrsekDev.Blazor.Mobx.Abstractions.Components;
using VrsekDev.Blazor.Mobx.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.StoreAccessors
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

        public override string ToString()
        {
            return componentName;
        }
    }
}
