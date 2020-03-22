using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Samples
{
    public interface ICounterStore
    {
        int CurrentCount { get; set; }
    }
}
