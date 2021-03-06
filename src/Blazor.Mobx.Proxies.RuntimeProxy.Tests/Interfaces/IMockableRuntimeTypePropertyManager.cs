﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Interfaces
{
    internal interface IMockableRuntimeTypePropertyManager : IRuntimeProxyManager
    {
        object GetValue(string name);

        void SetValue(string name, object value);
    }
}
