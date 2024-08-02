using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Describes a vertex element as a vector of integers.
/// </summary>
public class VertexElementInteger : IVertexElement
{
    private static readonly Dictionary<IntegerSourceType, int> SourceTypeSizes = new()
    {
        { IntegerSourceType.Byte, 1 },
        { IntegerSourceType.Short, 2 },
        { IntegerSourceType.Int, 4 },
        { IntegerSourceType.UnsignedByte, 1 },
        { IntegerSourceType.UnsignedShort, 2 },
        { IntegerSourceType.UnsignedInt, 4 }
    };

    private readonly int _location;
    private readonly int _numberOfComponents;
    private readonly IntegerSourceType _sourceType;

    /// <summary>
    /// Creates a new vertex element description for a vector of integers.
    /// </summary>
    /// <param name="location">Location at which this vertex element is accessible in the vertex shader.</param>
    /// <param name="numberOfComponents">Number of components of the described vector in the range between one and four.</param>
    /// <param name="sourceType">Concrete data type stored.</param>
    public VertexElementInteger(int location, int numberOfComponents, IntegerSourceType sourceType = IntegerSourceType.Int)
    {
        _location = location;
        _numberOfComponents = numberOfComponents;
        _sourceType = sourceType;
    }

    /// <inheritdoc cref="IVertexElement"/>
    public void ApplyDefinition(int strideSize, int offset, int divisor)
    {
        GL.EnableVertexAttribArray(_location);
        GL.VertexAttribIPointer(_location, _numberOfComponents, (VertexAttribIntegerType)_sourceType, strideSize, new IntPtr(offset));
        GL.VertexAttribDivisor(_location, divisor);
    }

    /// <summary>
    /// Gets the number of bytes required to represent this vertex element in memory.
    /// </summary>
    public int Size => _numberOfComponents * SourceTypeSizes[_sourceType];
}