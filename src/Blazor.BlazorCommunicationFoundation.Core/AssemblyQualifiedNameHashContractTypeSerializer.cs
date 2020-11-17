using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation
{
    public class AssemblyQualifiedNameHashContractTypeSerializer : IContractTypeSerializer
    {
        public string GenerateIdentifier(Type type)
        {
            string contractIdentifier = type.AssemblyQualifiedName;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(contractIdentifier);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
            }
        }
    }
}
