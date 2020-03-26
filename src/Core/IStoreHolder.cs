﻿using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx
{
    internal interface IStoreHolder<TStore>
        where TStore : class
    {
        MethodInterceptions StoreReactables { get; }
        IObservableProperty RootObservableProperty { get; }
        IStoreDependencyInjector<TStore> DependencyInjector { get; }

        event EventHandler<ObservablePropertyStateChangedEventArgs> StatePropertyChangedEvent;
        event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;
        event EventHandler<BatchObservableChangeEventArgs> BatchObservableChangeEvent;

        IObservableProperty CreateObservableProperty(Type type);
    }
}
