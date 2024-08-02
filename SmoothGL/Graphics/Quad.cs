using OpenTK.Mathematics;
using SmoothGL.Graphics.Geometry;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a quadratic finite plane orthogonal to the z-axis with unit size.
/// </summary>
public class Quad : GraphicsResource
{
    private readonly VertexArray _vertexArray;
    private readonly VertexBuffer _vertexBuffer;

    /// <summary>
    /// Creates a new quad in graphics memory.
    /// </summary>
    public Quad()
    {
        var declaration = new VertexDeclaration(
            new VertexElementFloat(0, 4)
        );
        Vector4[] data = { new(1, 1, 0, 1), new(-1, 1, 0, 1), new(1, -1, 0, 1), new(-1, -1, 0, 1) };

        _vertexBuffer = new VertexBuffer(4, declaration, BufferUsage.Static);
        _vertexBuffer.SetData(data);
        _vertexArray = new VertexArray(_vertexBuffer);
    }

    protected override string ResourceName => "Quad";

    /// <summary>
    /// Draws this quad with the shader program that is currently active.
    /// </summary>
    public void Draw()
    {
        _vertexArray.Draw(Primitive.TriangleStrip);
    }

    protected override void FreeResources()
    {
        _vertexBuffer.Dispose();
        _vertexArray.Dispose();
    }
}