using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Samples
{
    public interface ITodoStore
    {
        public int Value { get; set; }

        public IObservableCollection<string> Values { get; set; }
    }
}
