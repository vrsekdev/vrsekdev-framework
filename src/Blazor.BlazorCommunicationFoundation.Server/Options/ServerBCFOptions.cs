﻿using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options
{
    public class ServerBCFOptions : BCFOptions
    {
        public IServerContractCollection Contracts { get; internal set; }
    }
}