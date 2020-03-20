using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IPropertyAccessedSubscriber
    {
        void OnPropertyAccessed(PropertyAccessedArgs propertyAccessed);
    }
}
