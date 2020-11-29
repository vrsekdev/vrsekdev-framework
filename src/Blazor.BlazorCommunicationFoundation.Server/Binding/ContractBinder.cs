using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding
{
    public class ContractBinder : IContractBinder
    {
        private readonly IContractTypeBindingSerializer contractTypeBindingSerializer;
        private readonly IContractMethodBindingSerializer contractMethodBindingSerializer;

        private Dictionary<string, ContractMethodBinding> bindings = new Dictionary<string, ContractMethodBinding>();

        public ContractBinder(
            IContractTypeBindingSerializer contractTypeBindingSerializer,
            IContractMethodBindingSerializer contractMethodBindingSerializer)
        {
            this.contractTypeBindingSerializer = contractTypeBindingSerializer;
            this.contractMethodBindingSerializer = contractMethodBindingSerializer;
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
            string typeIdentifier = contractTypeBindingSerializer.GenerateIdentifier(contractType);
            string methodIdentifier = contractMethodBindingSerializer.GenerateIdentifier(methodInfo);

            string relativePath = $"{typeIdentifier}/{methodIdentifier}";

            bindings[relativePath] = new ContractMethodBinding(contractType, methodInfo);
        }

        IReadOnlyDictionary<string, ContractMethodBinding> IContractBinder.GetBindings()
        {
            return new ReadOnlyDictionary<string, ContractMethodBinding>(bindings);
        }
    }
}
