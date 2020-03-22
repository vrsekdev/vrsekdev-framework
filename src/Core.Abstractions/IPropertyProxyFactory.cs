﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public interface IPropertyProxyFactory
    {
        IPropertyProxy Create(IObservableProperty observableProperty, bool readOnly = false);
    }
}
