using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.PropertyObservers
{
    internal interface IPropertyObserverFactory
    {
        PropertyObserver<T> Create<T>() where T : class;
    }
}
