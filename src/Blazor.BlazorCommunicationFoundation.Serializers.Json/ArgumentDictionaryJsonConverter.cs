using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.Json
{
    class ArgumentDictionaryJsonConverter : JsonConverter<ArgumentDictionary>
    {
        private readonly Dictionary<string, Type> argumentMapping;

        public ArgumentDictionaryJsonConverter(
            Dictionary<string, Type> argumentMapping)
        {
            this.argumentMapping = argumentMapping;
        }

        public override ArgumentDictionary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
            }

            ArgumentDictionary arguments = new ArgumentDictionary();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return arguments;
                }

                var argumentName = reader.GetString();
                if (string.IsNullOrWhiteSpace(argumentName))
                {
                    throw new JsonException("Failed to get property name");
                }

                Type argumentType = argumentMapping[argumentName];

                object value = JsonSerializer.Deserialize(ref reader, argumentType, options);

                arguments.Add(argumentName, value);
            }

            return arguments;
        }

        public override void Write(Utf8JsonWriter writer, ArgumentDictionary value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
