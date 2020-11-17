using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public class BCFOptionsBuilder : IOptionsBuilder<BCFOptions>
    {
        private Type serializerType;

        public void UseSerializer(Type type)
        {
            serializerType = type;
        }

        public BCFOptions Build()
        {
            BCFOptions options = new BCFOptions();
            options.SerializerType = serializerType ?? options.SerializerType;

            return options;
        }
    }
}
