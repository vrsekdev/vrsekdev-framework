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
        private static Dictionary<Type, (SerializeDelegate, DeserializeDelegate)> deserializeCallCache = new Dictionary<Type, (SerializeDelegate, DeserializeDelegate)>();

        private readonly Dictionary<string, Type> argumentMapping;

        public ArgumentDictionaryFormatter(
            Dictionary<string, Type> argumentMapping)
        {
            this.argumentMapping = argumentMapping;
        }

        public void Serialize(ref MessagePackWriter writer, ArgumentDictionary arguments, MessagePackSerializerOptions options)
        {
            if (arguments == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(arguments.Count * 2);

            foreach (var argument in argumentMapping)
            {
                writer.WriteString(Encoding.UTF8.GetBytes(argument.Key));

                Type argumentType = argument.Value;
                object value = arguments[argument.Key];

                if (value == null)
                {
                    writer.WriteNil();
                    continue;
                }

                Serialize(argumentType, ref writer, value, options);
            }
        }

        public ArgumentDictionary Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            ArgumentDictionary value = new ArgumentDictionary();

            int count = reader.ReadArrayHeader();
            for (int i = 0; i < count / 2; i++)
            {
                string argumentName = reader.ReadString();
                Type argumentType = argumentMapping[argumentName];

                options.Security.DepthStep(ref reader);
                try
                {
                    if (reader.TryReadNil())
                    {
                        value.Add(argumentName, null);
                        continue;
                    }

                    object argumentValue = Deserialize(argumentType, ref reader, options);
                    value.Add(argumentName, argumentValue);
                }
                finally
                {
                    reader.Depth--;
                }
            }

            return value;
        }

        private static void Serialize(Type type, ref MessagePackWriter writer, object value, MessagePackSerializerOptions options)
        {
            if (!deserializeCallCache.TryGetValue(type, out (SerializeDelegate Serialize, DeserializeDelegate) delegates) || delegates.Serialize == null)
            {
                MethodInfo method = typeof(ArgumentDictionaryFormatter).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .Single(x => x.Name == nameof(Serialize) && x.ContainsGenericParameters)
                    .MakeGenericMethod(type);

                var writerParam = Expression.Parameter(typeof(MessagePackWriter).MakeByRefType(), "writer");
                var valueParam = Expression.Parameter(typeof(object), "value");
                var optionsParam = Expression.Parameter(typeof(MessagePackSerializerOptions), "options");
                var call = Expression.Call(method, writerParam, valueParam, optionsParam);
                var expression = Expression.Lambda<SerializeDelegate>(call, writerParam, valueParam, optionsParam);
                delegates.Serialize = expression.Compile();

                deserializeCallCache[type] = delegates;
            }

            delegates.Serialize(ref writer, value, options);
        }

        private static object Deserialize(Type type, ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (!deserializeCallCache.TryGetValue(type, out (SerializeDelegate, DeserializeDelegate Deserialize) delegates) || delegates.Deserialize == null)
            {
                MethodInfo method = typeof(ArgumentDictionaryFormatter).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .Single(x => x.Name == nameof(Deserialize) && x.ContainsGenericParameters)
                    .MakeGenericMethod(type);

                var readerParam = Expression.Parameter(typeof(MessagePackReader).MakeByRefType(), "reader");
                var optionsParam = Expression.Parameter(typeof(MessagePackSerializerOptions), "options");
                var call = Expression.Call(method, readerParam, optionsParam);
                var expression = Expression.Lambda<DeserializeDelegate>(call, readerParam, optionsParam);
                delegates.Deserialize = expression.Compile();

                deserializeCallCache[type] = delegates;
            }

            return delegates.Deserialize(ref reader, options);
        }


        private static void Serialize<T>(ref MessagePackWriter writer, object value, MessagePackSerializerOptions options)
        {
            IMessagePackFormatter<T> formatter = options.Resolver.GetFormatter<T>();
            if (formatter == null)
            {
                formatter = ContractlessStandardResolver.Instance.GetFormatter<T>();
            }

            formatter.Serialize(ref writer, (T)value, options);
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

        delegate void SerializeDelegate(ref MessagePackWriter writer, object value, MessagePackSerializerOptions options);
        delegate object DeserializeDelegate(ref MessagePackReader reader, MessagePackSerializerOptions options);
    }
}
