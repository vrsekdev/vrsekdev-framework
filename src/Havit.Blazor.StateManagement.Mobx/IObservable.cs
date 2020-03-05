using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    public interface IObservable
    {
        ObservableType ObservableType { get; }
    }
}
