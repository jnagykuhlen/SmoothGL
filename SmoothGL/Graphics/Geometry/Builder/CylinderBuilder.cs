using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Geometry.Builder;

/// <summary>
/// Represents a builder that constructs a unit cylinder aligned along the y-axis with specified level of detail.
/// </summary>
public class CylinderBuilder(int slices = 16) : IGeometryBuilder
{
    /// <summary>
    /// Builds a unit cylinder stored in memory.
    /// </summary>
    /// <returns>Unit cylinder.</returns>
    public MeshData Build()
    {
        var numberOfVertices = 4 * slices;
        var numberOfIndices = 6 * (slices - 2) + 6 * slices;

        var positions = new Vector3[numberOfVertices];
        var normals = new Vector3[numberOfVertices];
        var indices = new uint[numberOfIndices];

        var deltaTheta = MathHelper.TwoPi / slices;
        var index = 0;

        Vector3[] positionMasks =
        [
            new(1, -1, 1),
            new(1, 1, 1),
            new(1, -1, 1),
            new(1, 1, 1)
        ];

        Vector3[] normalMasks =
        [
            new(0, -1, 0),
            new(0, 1, 0),
            new(1, 0, 1),
            new(1, 0, 1)
        ];

        for (var i = 0; i < 4; ++i)
        {
            var positionMask = positionMasks[i];
            var normalMask = normalMasks[i];

            for (var slice = 0; slice < slices; ++slice)
            {
                var theta = slice * deltaTheta;
                var x = (float)Math.Sin(theta);
                var z = (float)Math.Cos(theta);

                positions[index] = new Vector3(x, 1.0f, z) * positionMask;
                normals[index] = new Vector3(x, 1.0f, z) * normalMask;

                ++index;
            }
        }

        index = 0;

        for (var i = 0; i < 2; ++i)
        {
            var offset = i * slices;

            for (var slice = 2; slice < slices; ++slice)
            {
                indices[index++] = (uint)(offset + 0);
                indices[index++] = (uint)(offset + slice - 1 + i);
                indices[index++] = (uint)(offset + slice - i);
            }
        }

        for (var slice = 0; slice < slices; ++slice)
        {
            indices[index++] = (uint)(2 * slices + (slice + 1) % slices);
            indices[index++] = (uint)(2 * slices + slice);
            indices[index++] = (uint)(3 * slices + slice);

            indices[index++] = (uint)(2 * slices + (slice + 1) % slices);
            indices[index++] = (uint)(3 * slices + slice);
            indices[index++] = (uint)(3 * slices + (slice + 1) % slices);
        }

        return new MeshData(positions, normals, null, indices);
    }
}