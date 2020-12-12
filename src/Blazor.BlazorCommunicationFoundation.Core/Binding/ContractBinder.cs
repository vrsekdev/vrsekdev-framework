using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Binding
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

        public string GetRequestPath(Type contractType, MethodInfo methodInfo)
        {
            string relativePath = GetRelativePath(contractType, methodInfo);

            return TransformToRequestPath(relativePath);
        }

        public bool IsPathFamiliar(string requestPath)
        {
            return bindings.ContainsKey(TransformFromRequestPath(requestPath));
        }

        public IReadOnlyDictionary<string, ContractMethodBinding> GetBindings()
        {
            return bindings;
        }

        internal void AddContractBinding(Type contractType)
        {
            MethodInfo[] contractMethods = contractType.GetMethods();
            foreach (MethodInfo contractMethod in contractMethods)
            {
                string relativePath = GetRelativePath(contractType, contractMethod);
                bindings[relativePath] = new ContractMethodBinding(contractType, contractMethod);
            }
        }

        private string GetRelativePath(Type contractType, MethodInfo methodInfo)
        {
            string typeIdentifier = contractTypeBindingSerializer.GenerateIdentifier(contractType);
            string methodIdentifier = contractMethodBindingSerializer.GenerateIdentifier(methodInfo);

            return $"{typeIdentifier}/{methodIdentifier}";
        }

        private static string TransformFromRequestPath(string requestPath)
        {
            if (requestPath.Length > 0 && requestPath[0] == '/')
            {
                return requestPath.Substring(1);
            }

            return requestPath;
        }

        private static string TransformToRequestPath(string relativePath)
        {
            return "/" + relativePath;
        }
    }
}
