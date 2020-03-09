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
        int CountElements { get; }

        void OverwriteElements(IEnumerable source);
        void Reset();
    }

    public interface IObservableCollection<T> : IObservableCollection, IList<T>
    {
        void AddRange(IEnumerable<T> items);
    }
}
