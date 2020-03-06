using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Models
{
    public interface IObservableArray : IEnumerable
    {
    }

    public interface IObservableArray<T> : IObservableArray, IList<T>
    {
        void AddRange(IEnumerable<T> items);
    }
}
