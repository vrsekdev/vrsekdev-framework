using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    internal class ClientBCFContractScope : IContractScope
    {
        public HashSet<Type> ContractTypes { get; internal set; }

        public Type HttpClientResolverType { get; internal set; }
        public object HttpClientArgs { get; internal set; }
    }
}
