using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Binding
{
    internal class MethodSignatureContractMethodBindingSerializer : IContractMethodBindingSerializer
    {
        public string GenerateIdentifier(MethodInfo methodInfo)
        {
            StringBuilder signatureBuilder = GenerateSignature(methodInfo.Name, methodInfo.GetParameters().Select(x => x.ParameterType));

            return signatureBuilder.ToString();
        }

        private StringBuilder GenerateSignature(string methodName, IEnumerable<Type> arguments)
        {
            StringBuilder stringBuilder = new StringBuilder(methodName.Length + arguments.Sum(x => x.Name.Length + 2) /* join + brackets */);

            stringBuilder.Append(methodName);
            stringBuilder.Append("(");
            stringBuilder.Append(String.Join(", ", arguments.Select(x => x.Name)));
            stringBuilder.Append(")");

            return stringBuilder;
        }
    }
}
