using System.Text.Json;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which deserializes objects of arbitrary type from a stream.
/// This reader is based on .NET JSON serialization.
/// </summary>
public class SerializationReader : IContentReader<object>
{
    /// <summary>
    /// Deserializes an object from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested.</param>
    /// <param name="contentProvider">Content provider used to load additional data.</param>
    /// <returns>The read object.</returns>
    public object Read(Stream stream, Type requestedType, IContentProvider contentProvider) =>
        JsonSerializer.Deserialize(stream, requestedType) ?? throw new ContentLoadException("Deserialized null.", stream, typeof(object));

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => true;
}