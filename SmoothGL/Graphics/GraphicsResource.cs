namespace SmoothGL.Graphics;

/// <summary>
///     Defines an abstract resource in graphics memory.
/// </summary>
public abstract class GraphicsResource : IDisposable
{
    /// <summary>
    ///     Creates a new graphics resource.
    /// </summary>
    protected GraphicsResource()
    {
        Disposed = false;
    }

    /// <summary>
    ///     Override this method to return the name of the resource.
    /// </summary>
    protected virtual string ResourceName => "GraphicsResource";

    /// <summary>
    ///     Gets a value indicating whether this resource has been disposed.
    /// </summary>
    public bool Disposed { get; private set; }

    /// <summary>
    ///     Disposes this resource, freeing all allocated graphics memory.
    /// </summary>
    public void Dispose()
    {
        if (!Disposed)
        {
            FreeResources();
            GC.SuppressFinalize(this);
            Disposed = true;
        }
    }

    /// <summary>
    ///     Throws an ObjectDisposedException in case this resource is already disposed.
    /// </summary>
    protected void CheckDisposed()
    {
        if (Disposed)
            throw new ObjectDisposedException(ResourceName, "The object is already disposed.");
    }

    /// <summary>
    ///     Override this method to free unmanaged resources.
    /// </summary>
    protected virtual void FreeResources()
    {
    }

    ~GraphicsResource()
    {
        if (!Disposed)
            try
            {
                Dispose();
            }
            catch (Exception exception)
            {
                throw new GraphicsResourceNotDisposedException(
                    string.Format(
                        "{0} has not been disposed before finalization and cannot be disposed automatically. " +
                        "Make sure that all graphics resources are disposed manually to avoid memory leaks.",
                        ResourceName
                    ),
                    exception
                );
            }
    }
}