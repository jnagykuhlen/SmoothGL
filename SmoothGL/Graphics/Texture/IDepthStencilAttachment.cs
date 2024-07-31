namespace SmoothGL.Graphics;

/// <summary>
/// Represents a handle to a storage object which can be attached to a custom frame buffer
/// such that depth and stencil testing is performed on it.
/// </summary>
public interface IDepthStencilAttachment
{
    /// <summary>
    /// Attaches the underlying storage object to the currently bound frame buffer.
    /// This method is not required to be called by client code.
    /// </summary>
    void Attach();
}