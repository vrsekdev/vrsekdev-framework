﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal interface IObservable
    {
        ObservableType ObservableType { get; }
    }
}
