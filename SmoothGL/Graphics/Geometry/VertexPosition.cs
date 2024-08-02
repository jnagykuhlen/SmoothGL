using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Defines a vertex by its position.
/// </summary>
public record struct VertexPosition(Vector3 Position)
{
    /// <summary>
    /// Gets the corresponding vertex declaration. The position is accessible at location 0
    /// in the vertex shader.
    /// </summary>
    public static readonly VertexDeclaration VertexDeclaration = new(new VertexElementFloat(0, 3));
}