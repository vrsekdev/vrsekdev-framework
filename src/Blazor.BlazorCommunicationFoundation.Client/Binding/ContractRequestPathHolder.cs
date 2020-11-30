using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Binding
{
    internal class ContractRequestPathHolder : IContractRequestPathHolder
    {
        private readonly IContractTypeBindingSerializer typeBindingSerializer;
        private readonly IContractMethodBindingSerializer methodBindingSerializer;

        private readonly HashSet<string> knownRequestPaths = new HashSet<string>();

        public ContractRequestPathHolder(
            IContractTypeBindingSerializer typeBindingSerializer,
            IContractMethodBindingSerializer methodBindingSerializer)
        {
            this.typeBindingSerializer = typeBindingSerializer;
            this.methodBindingSerializer = methodBindingSerializer;
        }

        public string GetRequestPath(Type contractType, MethodInfo methodInfo)
        {
            string typeIdentifier = typeBindingSerializer.GenerateIdentifier(contractType);
            string methodIdentifier = methodBindingSerializer.GenerateIdentifier(methodInfo);

            return BindingHelper.CreateRequestPath(typeIdentifier, methodIdentifier);
        }

        public bool IsPathFamiliar(string path)
        {
            return knownRequestPaths.Contains(path);
        }

        internal void AddBindings(Type contractType)
        {
            foreach (MethodInfo methodInfo in contractType.GetMethods())
            {
                knownRequestPaths.Add(GetRequestPath(contractType, methodInfo));
            }
        }
    }
}
