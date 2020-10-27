using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions
{
    public interface IPropertyProxyFactory
    {
        IPropertyProxy Create(IObservableProperty observableProperty, bool readOnly = false);

        IPropertyProxy Create(IObservableProperty observableProperty, MethodInterceptions interceptions, bool readOnly = false);
    }
}
