using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
///     Describes a vertex element as a vector of single precision floating point numbers.
/// </summary>
public class VertexElementFloat : IVertexElement
{
    private static readonly Dictionary<FloatSourceType, int> _sourceTypeSizes;

    private readonly int _location;
    private readonly bool _normalize;
    private readonly int _numberOfComponents;
    private readonly FloatSourceType _sourceType;

    static VertexElementFloat()
    {
        _sourceTypeSizes = new Dictionary<FloatSourceType, int>(12);
        _sourceTypeSizes.Add(FloatSourceType.Byte, 1);
        _sourceTypeSizes.Add(FloatSourceType.Short, 2);
        _sourceTypeSizes.Add(FloatSourceType.Int, 4);
        _sourceTypeSizes.Add(FloatSourceType.UnsignedByte, 1);
        _sourceTypeSizes.Add(FloatSourceType.UnsignedShort, 2);
        _sourceTypeSizes.Add(FloatSourceType.UnsignedInt, 4);
        _sourceTypeSizes.Add(FloatSourceType.Half, 2);
        _sourceTypeSizes.Add(FloatSourceType.Float, 4);
        _sourceTypeSizes.Add(FloatSourceType.Double, 8);
    }

    /// <summary>
    ///     Creates a new vertex element description for a vector of 32-bit single precision floating point numbers.
    /// </summary>
    /// <param name="location">Location at which this vertex element is accessible in the vertex shader.</param>
    /// <param name="numberOfComponents">Number of components of the described vector in the range between one and four.</param>
    public VertexElementFloat(int location, int numberOfComponents)
        : this(location, numberOfComponents, FloatSourceType.Float)
    {
    }

    /// <summary>
    ///     Creates a new vertex element description for a vector of floats.
    ///     Stored integer values are interpreted as floats without normalization.
    /// </summary>
    /// <param name="location">Location at which this vertex element is accessible in the vertex shader.</param>
    /// <param name="numberOfComponents">Number of components of the described vector in the range between one and four.</param>
    /// <param name="sourceType">Concrete data type stored.</param>
    public VertexElementFloat(int location, int numberOfComponents, FloatSourceType sourceType)
        : this(location, numberOfComponents, sourceType, false)
    {
    }

    /// <summary>
    ///     Creates a new vertex element description for a vector of floats.
    /// </summary>
    /// <param name="location">Location at which this vertex element is accessible in the vertex shader.</param>
    /// <param name="numberOfComponents">Number of components of the described vector in the range between one and four.</param>
    /// <param name="sourceType">Concrete data type stored.</param>
    /// <param name="normalize">
    ///     Specifies whether stored integer variables should be normalized to the range between zero and
    ///     one.
    /// </param>
    public VertexElementFloat(int location, int numberOfComponents, FloatSourceType sourceType, bool normalize)
    {
        _location = location;
        _numberOfComponents = numberOfComponents;
        _sourceType = sourceType;
        _normalize = normalize;
    }

    /// <summary>
    ///     Commmunicates this vertex element definition to the GPU. This method is not required to be called by client code.
    /// </summary>
    /// <param name="strideSize">The length of a single vertex representation in memory, in bytes.</param>
    /// <param name="offset">The offset at which this element is placed, in bytes.</param>
    /// <param name="divisor">The divisor.</param>
    public void ApplyDefinition(int strideSize, int offset, int divisor)
    {
        GL.EnableVertexAttribArray(_location);
        GL.VertexAttribPointer(_location, _numberOfComponents, (VertexAttribPointerType)_sourceType, _normalize, strideSize, offset);
        GL.VertexAttribDivisor(_location, divisor);
    }

    /// <summary>
    ///     Gets the number of bytes required to represent this vertex element in memory.
    /// </summary>
    public int Size => _numberOfComponents * _sourceTypeSizes[_sourceType];
}