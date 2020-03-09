using Havit.Blazor.StateManagement.Mobx.Samples.Models;
using Havit.Blazor.StateManagement.Mobx;
using Havit.Blazor.StateManagement.Mobx.Attributes;
using Havit.Blazor.StateManagement.Mobx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Samples
{
    public interface IHomeStore
    {
        IObservableArray<ITodoItem> Todos { get; set; }

        int TodoValue { get; set; }

        int Value { get; set; }

        string Timer { get; set; }

        [Observable]
        IPerson Person { get; set; }
    }
}
