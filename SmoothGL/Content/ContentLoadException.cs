namespace SmoothGL.Content;

/// <summary>
/// Exception which is thrown when content cannot be read by the content manager.
/// </summary>
public class ContentLoadException : Exception
{
    /// <summary>
    /// Creates a new ContentLoadException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception which indicated that the read operation failed.</param>
    /// <param name="filePath">Path to the content file for which loading failed.</param>
    /// <param name="contentType">Requested content type for which loading failed.</param>
    public ContentLoadException(string message, Exception? innerException, string? filePath = null, Type? contentType = null)
        : base(message, innerException)
    {
        FilePath = filePath;
        ContentType = contentType;
    }

    /// <summary>
    /// Creates a new ContentLoadException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="stream">Stream of the content for which loading failed.</param>
    /// <param name="contentType">Requested content type for which loading failed.</param>
    public ContentLoadException(string message, Stream stream, Type contentType)
        : this(message, null, (stream as FileStream)?.Name, contentType)
    {
    }

    /// <summary>
    /// Gets the path to the content file for which loading failed.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Gets the requested content type for which loading failed.
    /// </summary>
    public Type? ContentType { get; }
}