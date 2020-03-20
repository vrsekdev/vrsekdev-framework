﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IObservableCollection : IEnumerable
    {
        bool ElementObserved { get; }

        Type ElementType { get; }
    }

    public interface IObservableCollection<T> : IObservableCollection, IList<T>
    {
        void AddRange(IEnumerable<T> items);
    }
}
