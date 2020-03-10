using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IObservableCollection : IEnumerable
    {
        Type ElementType { get; }
        bool ElementObserved { get; }

        void Reset();
    }

    public interface IObservableCollection<T> : IObservableCollection, IList<T>
    {
        void AddRange(IEnumerable<T> items);
    }
}
