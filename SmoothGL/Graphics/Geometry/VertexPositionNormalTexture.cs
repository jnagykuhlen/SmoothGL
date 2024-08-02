using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Defines a vertex by its position, normal and texture coordinate.
/// </summary>
public record struct VertexPositionNormalTexture(Vector3 Position, Vector3 Normal, Vector2 TextureCoordinate)
{
    /// <summary>
    /// Gets the corresponding vertex declaration. The position is accessible at location 0, the normal at
    /// location 1 and the texture coordinate at location 2 in the vertex shader.
    /// </summary>
    public static readonly VertexDeclaration VertexDeclaration = new(
        new VertexElementFloat(0, 3),
        new VertexElementFloat(1, 3),
        new VertexElementFloat(2, 2)
    );
}