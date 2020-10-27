using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public interface IObservableCollection : IEnumerable
    {
        bool ElementObserved { get; }

        Type ElementType { get; }
    }

    public interface IObservableCollection<T> : IObservableCollection, IList<T>
    {
        void AddDefaultElements(IEnumerable<T> items);

        void AddRange(IEnumerable<T> items);
    }
}
