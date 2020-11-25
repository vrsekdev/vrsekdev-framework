using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.MessagePack
{
    internal class ArgumentDictionaryFormatterResolver : IFormatterResolver
    {
        private static DynamicGenericResolver dynamicGenericResolver = DynamicGenericResolver.Instance;

        private readonly Dictionary<string, Type> argumentMapping;

        public ArgumentDictionaryFormatterResolver(
            Dictionary<string, Type> argumentMapping)
        {
            this.argumentMapping = argumentMapping;
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (typeof(T) == typeof(ArgumentDictionary))
            {
                return (IMessagePackFormatter<T>)new ArgumentDictionaryFormatter(argumentMapping);
            }

            return dynamicGenericResolver.GetFormatter<T>();
        }
    }
}
