using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObservableActionInvoker<TStore>
    {
        private readonly Action<TStore> action;

        public ObservableActionInvoker(Action<TStore> action)
        {
            this.action = action;
        }

        public void Invoke(TStore store)
        {
            action(store);
        }
    }
}
