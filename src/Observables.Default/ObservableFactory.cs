﻿using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Observables.Default
{
    internal class ObservableFactory : IObservableFactory
    {
        private readonly EventHandler<ObservablePropertyStateChangedEventArgs> observablePropertyStateChangedEventArgs;
        private readonly EventHandler<ObservableCollectionItemsChangedEventArgs> observableCollectionItemsChangedEventArgs;

        public ObservableFactory(
            EventHandler<ObservablePropertyStateChangedEventArgs> observablePropertyStateChangedEventArgs,
            EventHandler<ObservableCollectionItemsChangedEventArgs> observableCollectionItemsChangedEventArgs)
        {
            this.observablePropertyStateChangedEventArgs = observablePropertyStateChangedEventArgs;
            this.observableCollectionItemsChangedEventArgs = observableCollectionItemsChangedEventArgs;
        }

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return new ObservableProperty(
                type,
                observablePropertyStateChangedEventArgs,
                observableCollectionItemsChangedEventArgs);
        }

        public IObservableCollection<T> CreateObservableArray<T>()
        {
            return new ObservableCollection<T>(
                observablePropertyStateChangedEventArgs,
                observableCollectionItemsChangedEventArgs);
        }
    }
}
