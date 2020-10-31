using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public class InvocationRequestArgumentSerializer : IInvocationRequestArgumentSerializer
    {
        private readonly IInvocationSerializer invocationSerializer;

        public InvocationRequestArgumentSerializer(
            IInvocationSerializer invocationSerializer)
        {
            this.invocationSerializer = invocationSerializer;
        }

        public InvocationRequestArgument[] SerializeArguments(object[] arguments)
        {
            InvocationRequestArgument[] result = new InvocationRequestArgument[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                result[i] = SerializeArgument(arguments[i]);
            }

            return result;
        }

        public object[] DeserializeArguments(InvocationRequestArgument[] arguments)
        {
            object[] result = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                result[i] = DeserializeArgument(arguments[i]);
            }

            return result;
        }

        private InvocationRequestArgument SerializeArgument(object value)
        {
            if (value == null)
            {
                return new InvocationRequestArgument
                {
                    Value = null
                };
            }

            Type type = value.GetType();
            return new InvocationRequestArgument
            {
                BindingInfo = new ArgumentBindingInfo
                {
                    TypeName = type.AssemblyQualifiedName
                },
                Value = invocationSerializer.Serialize(type, value)
            };
        }

        private object DeserializeArgument(InvocationRequestArgument argument)
        {
            if (argument.Value == null)
            {
                return null;
            }

            Type type = Type.GetType(argument.BindingInfo.TypeName);

            return invocationSerializer.Deserialize(type, argument.Value);
        }
    }
}
