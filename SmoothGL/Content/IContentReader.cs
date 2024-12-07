namespace SmoothGL.Content;

/// <summary>
/// Defines an interface for content readers which load instances of the specified type from a stream.
/// </summary>
/// <typeparam name="T">Type of the objects this class reads.</typeparam>
public interface IContentReader<in T> where T : notnull
{
    /// <summary>
    /// Reads an instance of the reader's type from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="contentProvider">Content provider used to load additional data.</param>
    /// <returns>The read object.</returns>
    TRequested Read<TRequested>(Stream stream, IContentProvider contentProvider) where TRequested : T;

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    bool CanReadSubtypes { get; }
}