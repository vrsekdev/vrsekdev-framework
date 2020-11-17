using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Infrastructure
{
    public class ContractNotRegisteredException : Exception
    {
        public ContractNotRegisteredException(string contractIdentifier)
        {
            ContractIdentifier = contractIdentifier;
        }

        public String ContractIdentifier { get; }
    }
}
