using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.Json
{
    public class JsonInvocationSerializer : IInvocationSerializer
    {
        private static JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public string MediaType => "application/json";

        public void Serialize<T>(Stream stream, T instance)
        {
            JsonSerializer.Serialize(new Utf8JsonWriter(stream), instance, Options);
        }

        public byte[] Serialize(Type type, object instance)
        {
            return JsonSerializer.SerializeToUtf8Bytes(instance, type, Options);
        }

        public Task SerializeAsync(Stream stream, Type type, object instance)
        {
            return JsonSerializer.SerializeAsync(stream, instance, type, Options);
        }

        public object Deserialize(Type type, byte[] value)
        {
            return JsonSerializer.Deserialize(value, type, Options);
        }

        public async Task<T> DeserializeAsync<T>(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, Options);
        }

        public async Task<object> DeserializeAsync(Type type, Stream stream)
        {
            return await JsonSerializer.DeserializeAsync(stream, type, Options);
        }

        public async Task<ArgumentDictionary> DeserializeArgumentsAsync(Stream stream, Dictionary<string, Type> argumentMapping)
        {
            JsonSerializerOptions options = new JsonSerializerOptions(Options);
            options.Converters.Add(new ArgumentDictionaryJsonConverter(argumentMapping));

            return await JsonSerializer.DeserializeAsync<ArgumentDictionary>(stream, options);
        }
    }
}
