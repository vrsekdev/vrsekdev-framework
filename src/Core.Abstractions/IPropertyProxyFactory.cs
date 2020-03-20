﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IPropertyProxyFactory
    {
        IPropertyProxy Create(IObservableProperty observableProperty);
    }
}
