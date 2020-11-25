using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.MessagePack
{
    public class MessagePackInvocationSerializer : IInvocationSerializer
    {
        private MessagePackSerializerOptions options = ContractlessStandardResolver.Options;

        public Task SerializeAsync(Stream stream, Type type, object instance)
        {
            return MessagePackSerializer.SerializeAsync(type, stream, instance, options);
        }

        public void Serialize<T>(Stream stream, T instance)
        {
            MessagePackSerializer.SerializeAsync(stream, instance, options);
        }

        public byte[] Serialize(Type type, object instance)
        {
            return MessagePackSerializer.Serialize(type, instance, options);
        }

        public async Task<T> DeserializeAsync<T>(Stream stream)
        {
            return await MessagePackSerializer.DeserializeAsync<T>(stream, options);
        }

        public async Task<object> DeserializeAsync(Type type, Stream stream)
        {
            return await MessagePackSerializer.DeserializeAsync(type, stream, options);
        }

        public object Deserialize(Type type, byte[] value)
        {
            return MessagePackSerializer.Deserialize(type, value, options);
        }

        public async Task<ArgumentDictionary> DeserializeArgumentsAsync(Stream stream, Dictionary<string, Type> argumentMapping)
        {
            return await MessagePackSerializer.DeserializeAsync<ArgumentDictionary>(stream, options.WithResolver(new ArgumentDictionaryFormatterResolver(argumentMapping)));
        }
    }
}
