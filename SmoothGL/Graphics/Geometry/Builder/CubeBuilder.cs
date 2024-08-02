using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Geometry.Builder;

/// <summary>
/// Represents a builder that constructs a unit cube aligned to the coordinate axes.
/// </summary>
public class CubeBuilder : IGeometryBuilder
{
    /// <summary>
    /// Builds a unit cube stored in memory.
    /// </summary>
    /// <returns>Unit cube.</returns>
    public MeshData Build()
    {
        Vector3[] vertices =
        {
            new(-1, -1, -1),
            new(-1, 1, -1),
            new(1, -1, -1),
            new(1, 1, -1),
            new(1, -1, 1),
            new(1, 1, 1),
            new(-1, -1, 1),
            new(-1, 1, 1)
        };

        Vector3[] positions =
        {
            vertices[0], vertices[6], vertices[7],
            vertices[0], vertices[7], vertices[1],

            vertices[5], vertices[2], vertices[3],
            vertices[2], vertices[5], vertices[4],

            vertices[3], vertices[0], vertices[1],
            vertices[3], vertices[2], vertices[0],

            vertices[7], vertices[6], vertices[4],
            vertices[5], vertices[7], vertices[4],

            vertices[4], vertices[0], vertices[2],
            vertices[4], vertices[6], vertices[0],

            vertices[5], vertices[3], vertices[1],
            vertices[5], vertices[1], vertices[7]
        };

        return new MeshData(positions);
    }
}