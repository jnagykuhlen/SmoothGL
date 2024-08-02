using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Geometry.Builder;

/// <summary>
/// Represents a builder that constructs a quad, which is a quadratic finite plane orthogonal to the z-axis with unit
/// size.
/// </summary>
public class QuadBuilder : IGeometryBuilder
{
    /// <summary>
    /// Builds a quad stored in memory.
    /// </summary>
    /// <returns>Quad.</returns>
    public MeshData Build()
    {
        Vector3[] positions =
        [
            new(1, 1, 0),
            new(-1, 1, 0),
            new(1, -1, 0),
            new(-1, -1, 0)
        ];

        Vector3[] normals =
        [
            Vector3.UnitZ,
            Vector3.UnitZ,
            Vector3.UnitZ,
            Vector3.UnitZ
        ];

        Vector2[] textureCoordinates =
        [
            new(1, 1),
            new(0, 1),
            new(1, 0),
            new(0, 0)
        ];

        uint[] indices =
        [
            0, 1, 2,
            1, 3, 2
        ];

        return new MeshData(positions, normals, textureCoordinates, indices);
    }
}