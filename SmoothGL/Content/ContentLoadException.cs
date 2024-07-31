namespace SmoothGL.Content;

/// <summary>
///     Exception which is thrown when content cannot be read by the content manager.
/// </summary>
public class ContentLoadException : Exception
{
    /// <summary>
    ///     Creates a new ContentLoadException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception which indicated that the read operation failed.</param>
    /// <param name="filename">Path to the content file for which loading failed.</param>
    /// <param name="contentType">Requested content type for which loading failed.</param>
    public ContentLoadException(string message, Exception innerException, string filename, Type contentType)
        : base(message, innerException)
    {
        Filename = filename;
        ContentType = contentType;
    }

    /// <summary>
    ///     Creates a new ContentLoadException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="filename">Path to the content file for which loading failed.</param>
    /// <param name="contentType">Requested content type for which loading failed.</param>
    public ContentLoadException(string message, string filename, Type contentType)
        : this(message, null, filename, contentType)
    {
    }

    /// <summary>
    ///     Gets the path to the content file for which loading failed.
    /// </summary>
    public string Filename { get; }

    /// <summary>
    ///     Gets the requested content type for which loading failed.
    /// </summary>
    public Type ContentType { get; }
}