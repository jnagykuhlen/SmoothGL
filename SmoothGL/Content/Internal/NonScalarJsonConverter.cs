using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmoothGL.Content.Internal;

public class NonScalarJsonConverter<T>(int numberOfValues, Func<float[], T> factory) : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray || !reader.Read())
            throw new JsonException($"Expected start of array when reading type {typeof(T).Name}.");

        var values = new float[numberOfValues];
        for (var i = 0; i < numberOfValues; ++i)
        {
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException($"Expected number when reading type {typeof(T).Name}.");
            
            values[i] = reader.GetSingle();
            reader.Read();
        }

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException($"Expected end of array when reading type {typeof(T).Name}.");

        return factory(values);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        throw new NotImplementedException($"Serialization of type {typeof(T).Name} is not supported.");
}