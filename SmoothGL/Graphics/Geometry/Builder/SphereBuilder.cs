using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Geometry.Builder;

/// <summary>
/// Represents a builder that constructs a unit sphere with specified level of detail.
/// </summary>
public class SphereBuilder(int slices = 16, int stacks = 8) : IGeometryBuilder
{
    /// <summary>
    /// Builds a unit sphere stored in memory.
    /// </summary>
    /// <returns>Unit sphere.</returns>
    public MeshData Build()
    {
        var numberOfVertices = (slices + 1) * (stacks + 1);
        var numberOfIndices = slices * stacks * 6;

        var positions = new Vector3[numberOfVertices];
        var normals = new Vector3[numberOfVertices];
        var textureCoordinates = new Vector2[numberOfVertices];
        var indices = new uint[numberOfIndices];

        var deltaPhi = MathHelper.Pi / stacks;
        var deltaTheta = MathHelper.TwoPi / slices;
        var index = 0;

        for (var stack = 0; stack <= stacks; ++stack)
        {
            var phi = MathHelper.PiOver2 - stack * deltaPhi;
            var y = (float)Math.Sin(phi);
            var scale = -(float)Math.Cos(phi);

            for (var slice = 0; slice <= slices; ++slice)
            {
                var theta = slice * deltaTheta;
                var x = scale * (float)Math.Sin(theta);
                var z = scale * (float)Math.Cos(theta);

                positions[index] = new Vector3(x, y, z);
                normals[index] = new Vector3(x, y, z);
                textureCoordinates[index] = new Vector2(slice / (float)slices, stack / (float)stacks);

                index++;
            }
        }

        index = 0;
        var k = slices + 1;

        for (var stack = 0; stack < stacks; ++stack)
        for (var slice = 0; slice < slices; ++slice)
        {
            indices[index++] = (uint)((stack + 0) * k + slice);
            indices[index++] = (uint)((stack + 1) * k + slice);
            indices[index++] = (uint)((stack + 0) * k + slice + 1);

            indices[index++] = (uint)((stack + 0) * k + slice + 1);
            indices[index++] = (uint)((stack + 1) * k + slice);
            indices[index++] = (uint)((stack + 1) * k + slice + 1);
        }

        return new MeshData(positions, normals, textureCoordinates, indices);
    }
}