using OpenTK.Mathematics;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a builder that constructs a unit sphere with specified level of detail.
/// </summary>
public class SphereBuilder : IGeometryBuilder
{
    private const int DefaultSlices = 16;
    private const int DefaultStacks = 8;

    private readonly int _slices;
    private readonly int _stacks;

    /// <summary>
    /// Creates a new sphere builder with default parameters for the level of detail.
    /// </summary>
    public SphereBuilder()
        : this(DefaultSlices, DefaultStacks)
    {
    }

    /// <summary>
    /// Creates a new sphere builder with specified parameters for the level of detail.
    /// </summary>
    /// <param name="slices">The number of subdivisions around the y-axis.</param>
    /// <param name="stacks">The number of subdivisions along the y-axis.</param>
    public SphereBuilder(int slices, int stacks)
    {
        _slices = slices;
        _stacks = stacks;
    }

    /// <summary>
    /// Builds a unit sphere stored in memory.
    /// </summary>
    /// <returns>Unit sphere.</returns>
    public MeshData Build()
    {
        var numberOfVertices = (_slices + 1) * (_stacks + 1);
        var numberOfIndices = _slices * _stacks * 6;

        var positions = new Vector3[numberOfVertices];
        var normals = new Vector3[numberOfVertices];
        var texCoords = new Vector2[numberOfVertices];
        var indices = new uint[numberOfIndices];

        float phi;
        float theta;
        var dphi = MathHelper.Pi / _stacks;
        var dtheta = MathHelper.TwoPi / _slices;
        float x, y, z, sc;
        var index = 0;

        for (var stack = 0; stack <= _stacks; ++stack)
        {
            phi = MathHelper.PiOver2 - stack * dphi;
            y = (float)Math.Sin(phi);
            sc = -(float)Math.Cos(phi);

            for (var slice = 0; slice <= _slices; ++slice)
            {
                theta = slice * dtheta;
                x = sc * (float)Math.Sin(theta);
                z = sc * (float)Math.Cos(theta);

                positions[index] = new Vector3(x, y, z);
                normals[index] = new Vector3(x, y, z);
                texCoords[index] = new Vector2(slice / (float)_slices, stack / (float)_stacks);

                index++;
            }
        }

        index = 0;
        var k = _slices + 1;

        for (var stack = 0; stack < _stacks; ++stack)
        for (var slice = 0; slice < _slices; ++slice)
        {
            indices[index++] = (uint)((stack + 0) * k + slice);
            indices[index++] = (uint)((stack + 1) * k + slice);
            indices[index++] = (uint)((stack + 0) * k + slice + 1);

            indices[index++] = (uint)((stack + 0) * k + slice + 1);
            indices[index++] = (uint)((stack + 1) * k + slice);
            indices[index++] = (uint)((stack + 1) * k + slice + 1);
        }

        return new MeshData(positions, normals, texCoords, indices);
    }
}