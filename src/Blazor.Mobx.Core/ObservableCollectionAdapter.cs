using VrsekDev.Blazor.Mobx.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx
{
    internal class ObservableCollectionAdapter<T> : List<T>, IObservableCollection<T>
    {
        public ObservableCollectionAdapter(IEnumerable<T> elements) : base(elements)
        {
        }

        public Type ElementType => throw new NotImplementedException();
        public bool ElementObserved => throw new NotImplementedException();

        public void AddDefaultElements(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
