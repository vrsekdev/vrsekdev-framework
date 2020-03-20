using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class PropertyAccessedObserver : IObserver<PropertyAccessedArgs>
    {
        private readonly EventHandler<PropertyAccessedEventArgs> propertyAccessedEvent;
        private readonly Action disposeAction;

        private bool disposed;

        public PropertyAccessedObserver(
            EventHandler<PropertyAccessedEventArgs> propertyAccessedEvent,
            Action disposeAction)
        {
            this.propertyAccessedEvent = propertyAccessedEvent;
            this.disposeAction = disposeAction;
        }

        public void OnNext(PropertyAccessedArgs value)
        {
            propertyAccessedEvent?.Invoke(this, new PropertyAccessedEventArgs
            {
                PropertyProxy = value.PropertyProxy,
                PropertyName = value.PropertyName
            });
        }

        public void OnCompleted()
        {
            if (!disposed)
            {
                disposed = true;
                disposeAction();
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
