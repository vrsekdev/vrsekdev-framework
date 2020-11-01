using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.Mobx.Abstractions.Events;

namespace VrsekDev.Blazor.Mobx
{
    public class StoreSubscribers
    {
        private delegate ValueTask SubscriberAction(IStoreSubscriber action);

        private ConcurrentDictionary<WeakReference<IStoreSubscriber>, bool> subscribers = new ConcurrentDictionary<WeakReference<IStoreSubscriber>, bool>();

        public void Add(IStoreSubscriber subscriber)
        {
            subscribers.TryAdd(new WeakReference<IStoreSubscriber>(subscriber), true);
        }

        public void NotifyComputedValueChanged(ComputedValueChangedArgs args)
        {
            foreach (var subscriber in subscribers)
            {
                InvokeActionOrRemove(subscriber.Key, subscriber => subscriber.ComputedValueChangedAsync(args));
            }
        }

        public void NotifyPropertyStateChanged(ObservablePropertyStateChangedArgs args)
        {
            foreach (var subscriber in subscribers)
            {
                InvokeActionOrRemove(subscriber.Key, subscriber => subscriber.PropertyStateChangedAsync(args));
            }
        }

        public void NotifyCollectionItemsChanged(ObservableCollectionItemsChangedArgs args)
        {
            foreach (var subscriber in subscribers)
            {
                InvokeActionOrRemove(subscriber.Key, subscriber => subscriber.CollectionItemsChangedAsync(args));
            }
        }

        public void NotifyBatchObservableChanged(BatchObservableChangeArgs args)
        {
            foreach (var subscriber in subscribers)
            {
                InvokeActionOrRemove(subscriber.Key, subscriber => subscriber.BatchObservableChangedAsync(args));
            }
        }

        private async void InvokeActionOrRemove(WeakReference<IStoreSubscriber> subscriberReference, SubscriberAction action)
        {
            if (subscriberReference.TryGetTarget(out IStoreSubscriber subscriber))
            {
                await action(subscriber);
            }
            else
            {
                subscribers.TryRemove(subscriberReference, out _);
            }
        }
    }
}
