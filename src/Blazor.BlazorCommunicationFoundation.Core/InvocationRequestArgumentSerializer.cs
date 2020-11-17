using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation
{
    public class InvocationRequestArgumentSerializer : IInvocationRequestArgumentSerializer
    {
        private readonly IInvocationSerializer invocationSerializer;

        public InvocationRequestArgumentSerializer(
            IInvocationSerializer invocationSerializer)
        {
            this.invocationSerializer = invocationSerializer;
        }

        public InvocationRequestArgument[] SerializeArguments(ParameterInfo[] parameters, object[] arguments)
        {
            InvocationRequestArgument[] result = new InvocationRequestArgument[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                result[i] = SerializeArgument(parameters[i], arguments[i]);
            }

            return result;
        }

        public object[] DeserializeArguments(ParameterInfo[] parameters, InvocationRequestArgument[] arguments)
        {
            object[] result = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                result[i] = DeserializeArgument(parameters[i], arguments[i].Value);
            }

            return result;
        }

        private InvocationRequestArgument SerializeArgument(ParameterInfo parameter, object value)
        {
            ArgumentBindingInfo bindingInfo = new ArgumentBindingInfo
            {
                TypeName = parameter.ParameterType.AssemblyQualifiedName
            };

            return new InvocationRequestArgument
            {
                BindingInfo = bindingInfo,
                Value = invocationSerializer.Serialize(parameter.ParameterType, value)
            };
        }

        private object DeserializeArgument(ParameterInfo parameter, byte[] value)
        {
            return invocationSerializer.Deserialize(parameter.ParameterType, value);
        }
    }
}
