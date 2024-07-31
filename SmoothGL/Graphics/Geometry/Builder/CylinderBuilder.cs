using OpenTK.Mathematics;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a builder that constructs a unit cylinder aligned along the y-axis with specified level of detail.
/// </summary>
public class CylinderBuilder : IGeometryBuilder
{
    private const int DefaultSlices = 16;

    private readonly int _slices;

    /// <summary>
    /// Creates a new cylinder builder with default parameters for the level of detail.
    /// </summary>
    public CylinderBuilder()
        : this(DefaultSlices)
    {
    }

    /// <summary>
    /// Creates a new cylinder builder with specified parameters for the level of detail.
    /// </summary>
    /// <param name="slices">The number of subdivisions around the y-axis.</param>
    public CylinderBuilder(int slices)
    {
        _slices = slices;
    }

    /// <summary>
    /// Builds a unit cylinder stored in memory.
    /// </summary>
    /// <returns>Unit cylinder.</returns>
    public MeshData Build()
    {
        var numberOfVertices = 4 * _slices;
        var numberOfIndices = 6 * (_slices - 2) + 6 * _slices;

        var positions = new Vector3[numberOfVertices];
        var normals = new Vector3[numberOfVertices];
        var indices = new uint[numberOfIndices];

        var dtheta = MathHelper.TwoPi / _slices;
        var index = 0;

        Vector3[] positionMasks =
        {
            new(1, -1, 1),
            new(1, 1, 1),
            new(1, -1, 1),
            new(1, 1, 1)
        };

        Vector3[] normalMasks =
        {
            new(0, -1, 0),
            new(0, 1, 0),
            new(1, 0, 1),
            new(1, 0, 1)
        };

        for (var i = 0; i < 4; ++i)
        {
            var positionMask = positionMasks[i];
            var normalMask = normalMasks[i];

            for (var slice = 0; slice < _slices; ++slice)
            {
                var theta = slice * dtheta;
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
            var offset = i * _slices;

            for (var slice = 2; slice < _slices; ++slice)
            {
                indices[index++] = (uint)(offset + 0);
                indices[index++] = (uint)(offset + slice - 1 + i);
                indices[index++] = (uint)(offset + slice - i);
            }
        }

        for (var slice = 0; slice < _slices; ++slice)
        {
            indices[index++] = (uint)(2 * _slices + (slice + 1) % _slices);
            indices[index++] = (uint)(2 * _slices + slice);
            indices[index++] = (uint)(3 * _slices + slice);

            indices[index++] = (uint)(2 * _slices + (slice + 1) % _slices);
            indices[index++] = (uint)(3 * _slices + slice);
            indices[index++] = (uint)(3 * _slices + (slice + 1) % _slices);
        }

        return new MeshData(positions, normals, null, indices);
    }
}