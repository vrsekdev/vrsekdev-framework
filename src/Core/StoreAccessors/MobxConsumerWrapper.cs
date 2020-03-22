using Havit.Blazor.Mobx.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.StoreAccessors
{
    internal class MobxConsumerWrapper : IConsumerWrapper
    {
        private readonly WeakReference<BlazorMobxComponentBase> consumerReference;
        private readonly string componentName;

        public MobxConsumerWrapper(BlazorMobxComponentBase consumer)
        {
            this.consumerReference = new WeakReference<BlazorMobxComponentBase>(consumer);
            this.componentName = consumer.GetType().Name;
        }

        public Task ForceUpdate()
        {
            if (!consumerReference.TryGetTarget(out BlazorMobxComponentBase consumer))
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
            if (!consumerReference.TryGetTarget(out BlazorMobxComponentBase consumer))
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
