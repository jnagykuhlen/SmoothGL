namespace SmoothGL.Content;

/// <summary>
/// Represents a content reader which can read content into an existing object.
/// </summary>
/// <typeparam name="T">Type of the objects this class reads.</typeparam>
public interface IHotSwappingContentReader<in T> : IContentReader<T> where T : notnull
{
    /// <summary>
    /// Reads content from a stream into an object of the reader's type.
    /// </summary>
    /// <param name="existingObject">Existing object to read content into.</param>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="contentProvider">Content provider used to load additional data.</param>
    void ReadInto(T existingObject, Stream stream, IContentProvider contentProvider);
}