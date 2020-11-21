using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding
{
    public class ContractBinder : IContractBinder
    {
        private readonly IContractBindingSerializer bindingSerializer;

        private Dictionary<string, (Type, MethodInfo)> bindings = new Dictionary<string, (Type, MethodInfo)>();

        public ContractBinder(IContractBindingSerializer bindingSerializer)
        {
            this.bindingSerializer = bindingSerializer;
        }

        public Type BindContractType(string bindingIdentifier)
        {
            if (!bindings.TryGetValue(bindingIdentifier, out (Type, MethodInfo) binding))
            {
                throw new ContractNotRegisteredException(bindingIdentifier);
            }

            return binding.Item1;
        }

        public MethodInfo BindContractMethod(string bindingIdentifier)
        {
            if (!bindings.TryGetValue(bindingIdentifier, out (Type, MethodInfo) binding))
            {
                throw new ContractNotRegisteredException(bindingIdentifier);
            }

            return binding.Item2;
        }

        internal void AddMethodBinding(Type contractType, MethodInfo methodInfo)
        {
            string bindingIdentifier = bindingSerializer.GenerateIdentifier(contractType, methodInfo);

            bindings[bindingIdentifier] = (contractType, methodInfo);
        }
    }
}
