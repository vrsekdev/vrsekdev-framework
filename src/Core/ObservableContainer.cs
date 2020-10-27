using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx
{
    public class ObservableContainer : IObservableContainer
    {
        protected HashSet<string> subscribedProperties = new HashSet<string>();

        public virtual void OnPropertyAccessed(string propertyName)
        {
            if (!subscribedProperties.Contains(propertyName))
            {
                subscribedProperties.Add(propertyName);
            }
        }

        public virtual bool IsSubscribed(string propertyName)
        {
            return subscribedProperties.Contains(propertyName);
        }
    }
}
