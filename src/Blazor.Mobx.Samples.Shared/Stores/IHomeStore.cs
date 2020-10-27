using VrsekDev.Blazor.Mobx.Samples.Models;
using System;
using System.Linq;
using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Attributes;

namespace VrsekDev.Blazor.Mobx.Samples
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
