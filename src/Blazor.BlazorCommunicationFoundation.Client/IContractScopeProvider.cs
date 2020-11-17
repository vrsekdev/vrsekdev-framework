using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public interface IContractScopeProvider
    {
        IContractScope GetScope(Type contractType);
    }
}
