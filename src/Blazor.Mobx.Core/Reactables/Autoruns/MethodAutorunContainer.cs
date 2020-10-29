using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Reactables.Autoruns
{
    public class MethodAutorunContainer<TStore> : IInvokableReactable
    {
        private readonly Action<TStore> autorunAction;
        private readonly TStore store;

        public MethodAutorunContainer(
            Action<TStore> autorunAction,
            TStore store)
        {
            this.autorunAction = autorunAction;
            this.store = store;
        }

        public bool RequiresInitialInvoke()
        {
            return true;
        }

        public void Invoke()
        {
            autorunAction(store);
        }
    }
}
