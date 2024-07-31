using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a buffer storing index data, persistent in graphics memory.
/// </summary>
public class ElementBuffer : Buffer
{
    /// <summary>
    /// Creates a new element buffer.
    /// </summary>
    /// <param name="numberOfElements">Maximum number of indices stored in this buffer.</param>
    /// <param name="elementType">Specifies the integer data type used for indices.</param>
    /// <param name="usage">Hint for the driver concerning the frequency the data in this buffer is expected to change.</param>
    public ElementBuffer(int numberOfElements, ElementType elementType, BufferUsage usage)
        : base(numberOfElements * GetElementTypeSize(elementType), BufferTarget.ElementArrayBuffer, usage)
    {
        NumberOfElements = numberOfElements;
        ElementType = elementType;
    }

    protected override string ResourceName => "ElementBuffer";

    /// <summary>
    /// Gets the integer data type used for indices in this buffer.
    /// </summary>
    public ElementType ElementType { get; }

    /// <summary>
    /// Gets the maximum number of elements which can be stored in this buffer.
    /// </summary>
    public int NumberOfElements { get; private set; }

    /// <summary>
    /// Gets the size of a single element stored in this buffer, measured in bytes.
    /// </summary>
    public int ElementSize => GetElementTypeSize(ElementType);

    private static int GetElementTypeSize(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.UnsignedByte:
                return sizeof(byte);
            case ElementType.UnsignedShort:
                return sizeof(ushort);
            case ElementType.UnsignedInt:
                return sizeof(uint);
            default:
                return 0;
        }
    }

    /// <summary>
    /// Resizes this buffer in a way that it can store the specified number of indices. All index data in this buffer will
    /// be discarded.
    /// </summary>
    /// <param name="numberOfElements">Maximum number of indices stored in this buffer.</param>
    public new void Resize(int numberOfElements)
    {
        NumberOfElements = numberOfElements;
        base.Resize(numberOfElements * GetElementTypeSize(ElementType));
    }

    private void SetData<T>(T[] data, ElementType requestedElementType)
        where T : struct
    {
        if (ElementType != requestedElementType)
            throw new ArgumentException(string.Format("Element buffer expects indices of type {0} instead of {1}.", ElementType, requestedElementType));

        if (data.Length > NumberOfElements)
            throw new ArgumentException("Cannot set data that exceeds buffer size.");

        SetData(data, 0, data.Length * GetElementTypeSize(ElementType));
    }

    /// <summary>
    /// Uploads index data to the GPU. An ArgumentException is thrown if the index data type of this buffer is not
    /// UnsignedByte.
    /// </summary>
    /// <param name="data">Data to upload to this buffer.</param>
    public void SetData(byte[] data)
    {
        SetData(data, ElementType.UnsignedByte);
    }

    /// <summary>
    /// Uploads index data to the GPU. An ArgumentException is thrown if the index data type of this buffer is not
    /// UnsignedShort.
    /// </summary>
    /// <param name="data">Data to upload to this buffer.</param>
    public void SetData(ushort[] data)
    {
        SetData(data, ElementType.UnsignedShort);
    }

    /// <summary>
    /// Uploads index data to the GPU. An ArgumentException is thrown if the index data type of this buffer is not
    /// UnsignedInt.
    /// </summary>
    /// <param name="data">Data to upload to this buffer.</param>
    public void SetData(uint[] data)
    {
        SetData(data, ElementType.UnsignedInt);
    }
}