using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Options
{
    public class BCFOptionsBuilder : IOptionsBuilder<BCFOptions>
    {
        private ICollection<Type> serializerTypes = new HashSet<Type>();

        public void AddSerializer(Type type)
        {
            serializerTypes.Add(type);
        }

        public BCFOptions Build()
        {
            BCFOptions options = new BCFOptions();

            foreach (var serializerType in serializerTypes)
            {
                if (!options.InvocationSerializerTypes.Contains(serializerType))
                {
                    options.InvocationSerializerTypes.Add(serializerType);
                }
            }

            return options;
        }
    }
}
