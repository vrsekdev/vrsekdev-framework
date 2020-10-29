using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Reactables.Autoruns
{
    public class MethodAutorunContainer<TStore> : IInvokableReactable
    {
        private readonly Func<TStore, ValueTask> autorunAction;
        private readonly TStore store;

        public MethodAutorunContainer(
            Func<TStore, ValueTask> autorunAction,
            TStore store)
        {
            this.autorunAction = autorunAction;
            this.store = store;
        }

        public bool RequiresInitialInvoke()
        {
            return true;
        }

        public ValueTask InvokeAsync()
        {
            return autorunAction(store);
        }
    }
}
