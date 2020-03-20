using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class PropertyAccessedSubscriber : IPropertyAccessedSubscriber
    {
        private readonly EventHandler<PropertyAccessedEventArgs> propertyAccessedEvent;

        public PropertyAccessedSubscriber(
            EventHandler<PropertyAccessedEventArgs> propertyAccessedEvent)
        {
            this.propertyAccessedEvent = propertyAccessedEvent;
        }
        public void OnPropertyAccessed(PropertyAccessedArgs propertyAccessed)
        {
            propertyAccessedEvent?.Invoke(this, new PropertyAccessedEventArgs
            {
                PropertyProxy = propertyAccessed.PropertyProxy,
                PropertyName = propertyAccessed.PropertyName
            });
        }
    }
}
