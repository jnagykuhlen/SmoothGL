namespace SmoothGL.Graphics;

/// <summary>
///     Exception which is thrown if a graphics resource has not been disposed before finalization and cannot be disposed
///     by the finalizer,
///     e.g. because freeing resources is thread-dependent.
/// </summary>
public class GraphicsResourceNotDisposedException : Exception
{
    /// <summary>
    ///     Creates a new GraphicsResourceNotDisposedException.
    /// </summary>
    /// <param name="message">Message describing why finalization failed.</param>
    public GraphicsResourceNotDisposedException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Creates a new GraphicsResourceNotDisposedException.
    /// </summary>
    /// <param name="message">Message describing why finalization failed.</param>
    /// <param name="innerException">Inner exception causing this exception to be thrown.</param>
    public GraphicsResourceNotDisposedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}