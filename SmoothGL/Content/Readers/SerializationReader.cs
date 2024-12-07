using System.Text.Json;
using SmoothGL.Content.Internal;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which deserializes objects of arbitrary type from a stream.
/// This reader is based on .NET JSON serialization.
/// </summary>
public class SerializationReader : IContentReader<object>, IHotSwappingReader
{
    public TRequested Read<TRequested>(Stream stream, IContentProvider contentProvider) where TRequested : notnull =>
        JsonSerializer.Deserialize<TRequested>(stream, CommonJsonSerializerOptions.CaseInsensitive) ?? throw new ContentLoadException("Deserialized null.", stream, typeof(object));

    public void ReadInto(object existingObject, Stream stream, IContentProvider contentProvider) =>
        JsonDocument.Parse(stream).RootElement.Populate(existingObject);

    public bool CanReadSubtypes => true;
}