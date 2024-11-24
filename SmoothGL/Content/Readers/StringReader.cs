namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads a string from a stream.
/// </summary>
public class StringReader : IContentReader<string>
{
    /// <summary>
    /// Reads a string from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentProvider">Content provider used to load additional data.</param>
    /// <returns>The read object.</returns>
    public string Read(Stream stream, Type requestedType, IContentProvider contentProvider)
    {
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;
}