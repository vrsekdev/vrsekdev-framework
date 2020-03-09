using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObservableCollectionAdapter<T> : List<T>, IObservableCollection<T>
    {
        public ObservableCollectionAdapter(IEnumerable<T> elements) : base(elements)
        {
        }

        public Type ElementType => throw new NotImplementedException();
        public bool ElementObserved => throw new NotImplementedException();
        public int CountElements => throw new NotImplementedException();

        public void OverwriteElements(IEnumerable source)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
