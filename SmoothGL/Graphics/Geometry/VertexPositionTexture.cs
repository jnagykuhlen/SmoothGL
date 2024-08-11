using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Defines a vertex by its position and texture coordinate.
/// </summary>
public record struct VertexPositionTexture(Vector3 Position, Vector2 TextureCoordinate)
{
    /// <summary>
    /// Gets the corresponding vertex declaration. The position is accessible at location 0
    /// and the texture coordinate at location 1 in the vertex shader.
    /// </summary>
    public static readonly VertexDeclaration VertexDeclaration = new(
        new VertexElementFloat(0, 3),
        new VertexElementFloat(1, 2)
    );
}