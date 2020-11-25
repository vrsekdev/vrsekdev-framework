using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions
{
    public interface IInvocationSerializer
    {
        void Serialize<T>(Stream stream, T instance);

        Task SerializeAsync(Stream stream, Type type, object instance);

        byte[] Serialize(Type type, object instance);

        Task<T> DeserializeAsync<T>(Stream stream);

        Task<ArgumentDictionary> DeserializeArgumentsAsync(Stream stream, Dictionary<string, Type> argumentMapping);

        Task<object> DeserializeAsync(Type type, Stream stream);

        object Deserialize(Type type, byte[] value);
    }
}
