using OpenTK.Mathematics;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a builder that constructs a regular tetrahedron with edge length two.
/// </summary>
public class TetrahedronBuilder : IGeometryBuilder
{
    /// <summary>
    /// Builds a regular tetrahedron stored in memory.
    /// </summary>
    /// <returns>Regular tetrahedron.</returns>
    public MeshData Build()
    {
        var oneOverSqrtTwo = MathHelper.InverseSqrtFast(2.0f);

        Vector3[] vertices =
        {
            new(-1, 0, -oneOverSqrtTwo),
            new(1, 0, -oneOverSqrtTwo),
            new(0, -1, oneOverSqrtTwo),
            new(0, 1, oneOverSqrtTwo)
        };

        Vector3[] positions =
        {
            vertices[2], vertices[1], vertices[3],
            vertices[0], vertices[2], vertices[3],
            vertices[1], vertices[0], vertices[3],
            vertices[0], vertices[1], vertices[2]
        };

        return new MeshData(positions, null);
    }
}