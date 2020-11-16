using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Infrastructure
{
    public class ContractNotRegisteredException : Exception
    {
        public ContractNotRegisteredException(Type contractType)
        {
            ContractType = contractType;
        }

        public Type ContractType { get; }
    }
}
