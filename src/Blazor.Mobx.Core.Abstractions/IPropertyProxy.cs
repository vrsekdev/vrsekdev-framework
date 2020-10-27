using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions
{
    public interface IPropertyProxy : IObservableProxy
    {
        bool IsReadOnly { get; }

        IObservableProperty ObservableProperty { get; }
    }
}
