using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private Dictionary<string, ContractMethodBinding> bindings = new Dictionary<string, ContractMethodBinding>();

        public ContractBinder(IContractBindingSerializer bindingSerializer)
        {
            this.bindingSerializer = bindingSerializer;
        }

        public Type BindContractType(string bindingIdentifier)
        {
            if (!bindings.TryGetValue(bindingIdentifier, out ContractMethodBinding binding))
            {
                throw new ContractNotRegisteredException(bindingIdentifier);
            }

            return binding.ContractType;
        }

        public MethodInfo BindContractMethod(string bindingIdentifier)
        {
            if (!bindings.TryGetValue(bindingIdentifier, out ContractMethodBinding binding))
            {
                throw new ContractNotRegisteredException(bindingIdentifier);
            }

            return binding.ContractMethodInfo;
        }

        internal void AddMethodBinding(Type contractType, MethodInfo methodInfo)
        {
            string bindingIdentifier = bindingSerializer.GenerateIdentifier(contractType, methodInfo);

            bindings[bindingIdentifier] = new ContractMethodBinding(contractType, methodInfo);
        }

        IReadOnlyDictionary<string, ContractMethodBinding> IContractBinder.GetBindings()
        {
            return new ReadOnlyDictionary<string, ContractMethodBinding>(bindings);
        }
    }
}
