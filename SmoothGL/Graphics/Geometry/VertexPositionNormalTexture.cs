using OpenTK.Mathematics;

namespace SmoothGL.Graphics;

/// <summary>
///     Defines a vertex by its position, normal and texture coordinate.
/// </summary>
public struct VertexPositionNormalTexture
{
    /// <summary>
    ///     Gets the corresponding vertex declaration. The position is accessible at location 0, the normal at
    ///     location 1 and the texture coordinate at location 2 in the vertex shader.
    /// </summary>
    public static readonly VertexDeclaration VertexDeclaration;

    /// <summary>
    ///     The position of the vertex.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    ///     The normal vector of the vertex.
    /// </summary>
    public Vector3 Normal;

    /// <summary>
    ///     The texture coordinate of the vertex.
    /// </summary>
    public Vector2 TextureCoordinate;

    static VertexPositionNormalTexture()
    {
        VertexDeclaration = new VertexDeclaration(
            new VertexElementFloat(0, 3),
            new VertexElementFloat(1, 3),
            new VertexElementFloat(2, 2)
        );
    }

    /// <summary>
    ///     Creates a new vertex with specified position, normal and texture coordinate.
    /// </summary>
    /// <param name="position">Position of the vertex.</param>
    /// <param name="normal">Normal vector of the vertex.</param>
    /// <param name="textureCoordinate">Texture coordinate of the vertex.</param>
    public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
    {
        Position = position;
        Normal = normal;
        TextureCoordinate = textureCoordinate;
    }
}