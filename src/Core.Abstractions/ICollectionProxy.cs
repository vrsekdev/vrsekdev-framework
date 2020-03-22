using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public interface ICollectionProxy : IObservableProxy, IEnumerable
    {
        Type ElementType { get; }

        bool ElementObserved { get; }

        void Reset();
    }
}
