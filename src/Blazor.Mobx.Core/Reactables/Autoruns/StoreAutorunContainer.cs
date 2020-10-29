using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Reactables.Autoruns
{
    internal class StoreAutorunContainer<TStore> : IInvokableReactable
    {
        private readonly Lazy<Action> autorunActionLazy;


        public StoreAutorunContainer(
            MethodInfo storeAutorunMethod,
            TStore store)
        {
            autorunActionLazy = new Lazy<Action>(() => CreateDelegate(storeAutorunMethod, store));
        }

        private Action CreateDelegate(MethodInfo storeAutorunMethod, TStore store)
        {
            return (Action)Delegate.CreateDelegate(typeof(Action), store, storeAutorunMethod);
        }

        public bool RequiresInitialInvoke()
        {
            return true;
        }

        public ValueTask InvokeAsync()
        {
            autorunActionLazy.Value();

            return default;
        }
    }
}
