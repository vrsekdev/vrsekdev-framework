using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.MessagePack
{
    internal class ArgumentDictionaryFormatter : IMessagePackFormatter<ArgumentDictionary>
    {
        private static Dictionary<Type, DeserializeDelegate> deserializeCallCache = new Dictionary<Type, DeserializeDelegate>();

        private readonly Dictionary<string, Type> argumentMapping;

        public ArgumentDictionaryFormatter(
            Dictionary<string, Type> argumentMapping)
        {
            this.argumentMapping = argumentMapping;
        }

        public void Serialize(ref MessagePackWriter writer, ArgumentDictionary arguments, MessagePackSerializerOptions options)
        {
            // Serialization is done through common messagepack serialize to ensure compatability
            throw new NotImplementedException();
        }

        public ArgumentDictionary Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            ArgumentDictionary arguments = new ArgumentDictionary();

            var len = reader.ReadMapHeader();

            options.Security.DepthStep(ref reader);
            try
            {
                for (int i = 0; i < len; i++)
                {
                    string argumentName = reader.ReadString();
                    Type argumentType = argumentMapping[argumentName];

                    object value = Deserialize(argumentType, ref reader, options);

                    arguments.Add(argumentName, value);
                }
            }
            finally
            {
                reader.Depth--;
            }

            return arguments;
        }

        private static object Deserialize(Type type, ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (!deserializeCallCache.TryGetValue(type, out DeserializeDelegate deserialize))
            {
                MethodInfo method = typeof(ArgumentDictionaryFormatter).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .Single(x => x.Name == nameof(Deserialize) && x.ContainsGenericParameters)
                    .MakeGenericMethod(type);

                var readerParam = Expression.Parameter(typeof(MessagePackReader).MakeByRefType(), "reader");
                var optionsParam = Expression.Parameter(typeof(MessagePackSerializerOptions), "options");
                var call = Expression.Call(method, readerParam, optionsParam);
                var expression = Expression.Lambda<DeserializeDelegate>(call, readerParam, optionsParam);
                deserialize = expression.Compile();

                deserializeCallCache.Add(type, deserialize);
            }

            return deserialize(ref reader, options);
        }

        private static object Deserialize<T>(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            IMessagePackFormatter<T> formatter = options.Resolver.GetFormatter<T>();
            if (formatter == null)
            {
                formatter = ContractlessStandardResolver.Instance.GetFormatter<T>();
            }

            return formatter.Deserialize(ref reader, options);
        }

        delegate object DeserializeDelegate(ref MessagePackReader reader, MessagePackSerializerOptions options);
    }
}
