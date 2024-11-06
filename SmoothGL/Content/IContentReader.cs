namespace SmoothGL.Content;

/// <summary>
/// Defines an interface for content readers which load instances of the specified type from a stream.
/// </summary>
/// <typeparam name="T">Type of the objects this class reads.</typeparam>
public interface IContentReader<out T> where T : notnull
{
    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    bool CanReadSubtypes { get; }

    /// <summary>
    /// Reads an instance of the reader's type from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    T Read(Stream stream, Type requestedType, ContentManager contentManager);
}