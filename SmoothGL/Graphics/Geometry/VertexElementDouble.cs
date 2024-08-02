using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Describes a vertex element as a vector of double precision floating point numbers.
/// </summary>
public class VertexElementDouble : IVertexElement
{
    private readonly int _location;
    private readonly int _numberOfComponents;

    /// <summary>
    /// Creates a new vertex element description for a vector of doubles.
    /// </summary>
    /// <param name="location">Location at which this vertex element is accessible in the vertex shader.</param>
    /// <param name="numberOfComponents">Number of components of the described vector in the range between one and four.</param>
    public VertexElementDouble(int location, int numberOfComponents)
    {
        if (numberOfComponents is < 1 or > 4)
            throw new ArgumentOutOfRangeException(nameof(numberOfComponents), "The number of components must lie between one and four.");

        _location = location;
        _numberOfComponents = numberOfComponents;
    }

    /// <inheritdoc cref="IVertexElement"/>
    public void ApplyDefinition(int strideSize, int offset, int divisor)
    {
        GL.EnableVertexAttribArray(_location);
        GL.VertexAttribLPointer(_location, _numberOfComponents, VertexAttribDoubleType.Double, strideSize, new IntPtr(offset));
        GL.VertexAttribDivisor(_location, divisor);
    }

    public int Size => _numberOfComponents * 8;
}