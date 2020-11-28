using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding
{
    public class ContractMethodBinding
    {
        public ContractMethodBinding(Type contractType, MethodInfo contractMethodInfo)
        {
            ContractType = contractType;
            ContractMethodInfo = contractMethodInfo;
        }

        public Type ContractType { get; }

        public MethodInfo ContractMethodInfo { get; }
    }
}
