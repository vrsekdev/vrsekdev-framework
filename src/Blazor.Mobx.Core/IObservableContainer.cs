using VrsekDev.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx
{
    public interface IObservableContainer
    {
        void OnPropertyAccessed(string propertyName);

        bool IsSubscribed(string propertyName);
    }
}
