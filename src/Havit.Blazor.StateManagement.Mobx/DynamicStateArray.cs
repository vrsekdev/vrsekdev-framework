using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    /*public class DynamicStateArray : IObservable<CollectionItemsChangedArgs>
    {
        private readonly List<IObserver<CollectionItemsChangedArgs>> observers = new List<IObserver<CollectionItemsChangedArgs>>();

        public ObservableArray ObservableArray { get; }

        public DynamicStateArray(ObservableArray observableArray)
        {
            ObservableArray = observableArray;
        }

        public IDisposable Subscribe(IObserver<CollectionItemsChangedArgs> observer)
        {
            observers.Add(observer);

            return new ObserverDisposer(() => observers.Remove(observer));
        }

        private class ObserverDisposer : IDisposable
        {
            private readonly Action disposeAction;

            public ObserverDisposer(Action disposeAction)
            {
                this.disposeAction = disposeAction;
            }

            public void Dispose()
            {
                disposeAction();
            }
        }
    }*/
}
