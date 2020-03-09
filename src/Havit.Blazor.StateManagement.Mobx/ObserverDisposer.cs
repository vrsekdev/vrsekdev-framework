using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObserverDisposer : IDisposable
    {
        private readonly List<Action> disposeActions = new List<Action>();

        private bool disposed;

        public void AddDisposeAction(Action action)
        {
            disposeActions.Add(action);
        }

        public void AddDisposeAction(IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            disposeActions.Add(() => disposable.Dispose());
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                foreach (var disposeAction in disposeActions)
                {
                    disposeAction();
                }
            }
#if DEBUG
            else
            {
                throw new Exception("Already disposed.");
            }
#endif
        }
    }
}
