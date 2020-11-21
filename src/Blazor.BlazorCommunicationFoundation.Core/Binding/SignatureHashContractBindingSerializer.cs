using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Binding
{
    internal class SignatureHashContractBindingSerializer : IContractBindingSerializer
    {
        private readonly IContractTypeSerializer contractTypeSerializer;

        public SignatureHashContractBindingSerializer(
            IContractTypeSerializer contractTypeSerializer)
        {
            this.contractTypeSerializer = contractTypeSerializer;
        }

        public string GenerateIdentifier(Type contractType, MethodInfo methodInfo)
        {
            StringBuilder stringBuilder = new StringBuilder(contractTypeSerializer.GenerateIdentifier(contractType));

            StringBuilder signatureBuilder = GenerateSignature(methodInfo.Name, methodInfo.GetParameters().Select(x => x.ParameterType));
            stringBuilder.Append(".");
            stringBuilder.Append(signatureBuilder);

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
            }
        }

        private StringBuilder GenerateSignature(string methodName, IEnumerable<Type> arguments)
        {
            StringBuilder stringBuilder = new StringBuilder(methodName.Length + arguments.Sum(x => x.FullName.Length + 2) /* join + brackets */);

            stringBuilder.Append(methodName);
            stringBuilder.Append("(");
            stringBuilder.Append(String.Join(", ", arguments.Select(x => x.FullName)));
            stringBuilder.Append(")");

            return stringBuilder;
        }
    }
}
