using Havit.Blazor.Mobx.Samples.Models;
using System;
using System.Linq;
using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Attributes;

namespace Havit.Blazor.Mobx.Samples
{
    public interface IHomeStore
    {
        [Observable]
        IObservableCollection<ITodoItem> Todos { get; set; }

        int TodoValue { get; set; }

        int Value { get; set; }

        string Timer { get; set; }

        [Observable]
        IPerson Person { get; set; }
    }
}
