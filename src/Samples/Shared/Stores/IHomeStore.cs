using Havit.Blazor.StateManagement.Mobx.Samples.Models;
using System;
using System.Linq;
using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Observables.Default.Attributes;

namespace Havit.Blazor.StateManagement.Mobx.Samples
{
    public interface IHomeStore
    {
        IObservableCollection<ITodoItem> Todos { get; set; }

        int TodoValue { get; set; }

        int Value { get; set; }

        string Timer { get; set; }

        [Observable]
        IPerson Person { get; set; }
    }
}
