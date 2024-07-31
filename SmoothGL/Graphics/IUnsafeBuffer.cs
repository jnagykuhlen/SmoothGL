namespace SmoothGL.Graphics;

/// <summary>
/// Provides unsafe access to a buffer, allowing to modify its memory directly. This can result in runtime errors or
/// unintended behavior and should be avoided if possible.
/// </summary>
public interface IUnsafeBuffer
{
    /// <summary>
    /// Replaces a single data object in the buffer.
    /// </summary>
    /// <param name="data">Data object to upload. Only instances of value types (structs) are allowed as this parameter.</param>
    /// <param name="offset">The offset in bytes from the beginning of the buffer at which the data object should be stored.</param>
    void SetData(object data, int offset);

    /// <summary>
    /// Replaces a single data object in the buffer.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="data">Data object to upload.</param>
    /// <param name="offset">The offset in bytes from the beginning of the buffer at which the data object should be stored.</param>
    /// <param name="size">Size of the data object in bytes.</param>
    void SetData<T>(T data, int offset, int size) where T : struct;

    /// <summary>
    /// Replaces multiple data objects in the buffer.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="data">Array of sequential data objects to upload.</param>
    /// <param name="offset">The offset in bytes from the beginning of the buffer at which the data objects should be stored.</param>
    /// <param name="size">Size of the data objects in bytes.</param>
    void SetData<T>(T[] data, int offset, int size) where T : struct;
}