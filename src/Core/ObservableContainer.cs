using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx
{
    internal class ObservableContainer : IObservableContainer
    {
        private HashSet<string> subscribedProperties = new HashSet<string>();

        public void OnPropertyAccessed(string propertyName)
        {
            if (!subscribedProperties.Contains(propertyName))
            {
                subscribedProperties.Add(propertyName);
            }
        }

        public bool IsSubscribed(string propertyName)
        {
            return subscribedProperties.Contains(propertyName);
        }
    }
}
