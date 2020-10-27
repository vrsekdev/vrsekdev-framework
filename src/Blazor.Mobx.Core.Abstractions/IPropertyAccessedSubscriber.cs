using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions
{
    public interface IPropertyAccessedSubscriber
    {
        void OnPropertyAccessed(PropertyAccessedArgs propertyAccessed);
    }
}
