using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Models
{
    public class ObservableArrayAdapter<T> : List<T>, IObservableArray<T>
    {
        public ObservableArrayAdapter(IEnumerable<T> elements) : base(elements)
        {
        }
    }
}
