using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal interface IObservableContainer
    {
        void OnPropertyAccessed(string propertyName);

        bool IsSubscribed(string propertyName);
    }
}
