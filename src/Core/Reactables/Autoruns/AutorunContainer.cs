using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables.Autoruns
{
    internal class AutorunContainer<TStore> : IInvokableReactable
    {
        private readonly Lazy<Action> autorunActionLazy;

        private bool isInitialized;


        public AutorunContainer(
            MethodInfo storeAutorunMethod,
            TStore store)
        {
            autorunActionLazy = new Lazy<Action>(() => CreateDelegate(storeAutorunMethod, store));
        }

        private Action CreateDelegate(MethodInfo storeAutorunMethod, TStore store)
        {
            return (Action)Delegate.CreateDelegate(typeof(Action), store, storeAutorunMethod);
        }

        public bool ShouldInvoke()
        {
            // behave promiscous when not initialized
            return !isInitialized;
        }

        public void Invoke()
        {
            if (!isInitialized)
                isInitialized = true;

            autorunActionLazy.Value();
        }
    }
}
