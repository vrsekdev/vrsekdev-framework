using Havit.Blazor.StateManagement.Mobx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Samples
{
    public interface ITodoStore
    {
        public int Value { get; set; }

        public IObservableArray<string> Values { get; set; }
    }
}
