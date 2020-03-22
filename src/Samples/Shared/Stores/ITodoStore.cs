using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Samples
{
    public interface ITodoStore
    {
        int Value { get; set; }

        IObservableCollection<string> Values { get; set; }
    }
}
