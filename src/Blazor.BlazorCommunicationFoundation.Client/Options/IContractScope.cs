using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public interface IContractScope
    {
        HashSet<Type> ContractTypes { get; }

        Type HttpClientResolverType { get; }
        object HttpClientArgs { get; }
    }
}
