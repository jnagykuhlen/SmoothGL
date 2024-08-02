using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Describes a vertex element as a vector of single precision floating point numbers.
/// </summary>
public class VertexElementFloat : IVertexElement
{
    private static readonly Dictionary<FloatSourceType, int> SourceTypeSizes = new()
    {
        { FloatSourceType.Byte, 1 },
        { FloatSourceType.Short, 2 },
        { FloatSourceType.Int, 4 },
        { FloatSourceType.UnsignedByte, 1 },
        { FloatSourceType.UnsignedShort, 2 },
        { FloatSourceType.UnsignedInt, 4 },
        { FloatSourceType.Half, 2 },
        { FloatSourceType.Float, 4 },
        { FloatSourceType.Double, 8 }
    };

    private readonly int _location;
    private readonly bool _normalize;
    private readonly int _numberOfComponents;
    private readonly FloatSourceType _sourceType;

    /// <summary>
    /// Creates a new vertex element description for a vector of floats.
    /// </summary>
    /// <param name="location">Location at which this vertex element is accessible in the vertex shader.</param>
    /// <param name="numberOfComponents">Number of components of the described vector in the range between one and four.</param>
    /// <param name="sourceType">Concrete data type stored.</param>
    /// <param name="normalize">
    /// Specifies whether stored integer variables should be normalized to the range between zero and
    /// one.
    /// </param>
    public VertexElementFloat(int location, int numberOfComponents, FloatSourceType sourceType = FloatSourceType.Float, bool normalize = false)
    {
        _location = location;
        _numberOfComponents = numberOfComponents;
        _sourceType = sourceType;
        _normalize = normalize;
    }

    /// <inheritdoc cref="IVertexElement"/>
    public void ApplyDefinition(int strideSize, int offset, int divisor)
    {
        GL.EnableVertexAttribArray(_location);
        GL.VertexAttribPointer(_location, _numberOfComponents, (VertexAttribPointerType)_sourceType, _normalize, strideSize, offset);
        GL.VertexAttribDivisor(_location, divisor);
    }

    /// <summary>
    /// Gets the number of bytes required to represent this vertex element in memory.
    /// </summary>
    public int Size => _numberOfComponents * SourceTypeSizes[_sourceType];
}