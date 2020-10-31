using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    internal class MessagePackInvocationSerializer : IInvocationSerializer
    {
        private MessagePackSerializerOptions options = ContractlessStandardResolver.Options;

        public void Serialize<T>(Stream stream, T instance)
        {
            MessagePackSerializer.SerializeAsync(stream, instance, options);
        }

        public Task SerializeAsync(Stream stream, Type type, object instance)
        {
            return MessagePackSerializer.SerializeAsync(type, stream, instance, options);
        }

        public async Task<T> DeserializeAsync<T>(Stream stream)
        {
            return await MessagePackSerializer.DeserializeAsync<T>(stream, options);
        }
    }
}
