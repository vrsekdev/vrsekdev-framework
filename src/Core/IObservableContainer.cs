using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx
{
    public interface IObservableContainer
    {
        void OnPropertyAccessed(string propertyName);

        bool IsSubscribed(string propertyName);
    }
}
