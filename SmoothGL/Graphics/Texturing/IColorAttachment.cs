namespace SmoothGL.Graphics.Texturing;

/// <summary>
/// Represents a handle to a storage object which can be attached to a custom frame buffer
/// such that color values are written to it.
/// </summary>
public interface IColorAttachment
{
    /// <summary>
    /// Attaches the underlying storage object to the currently bound frame buffer at the specified index.
    /// This method is not required to be called by client code.
    /// </summary>
    /// <param name="index">Index at which the underlying storage is attached.</param>
    void Attach(int index);
}