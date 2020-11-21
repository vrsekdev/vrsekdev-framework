using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure
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
