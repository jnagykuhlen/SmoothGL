using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Mathematics;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a builder that constructs a unit sphere with specified level of detail.
    /// </summary>
    public class SphereBuilder : IGeometryBuilder
    {
        private const int DefaultSlices = 16;
        private const int DefaultStacks = 8;

        private int _slices;
        private int _stacks;

        /// <summary>
        /// Creates a new sphere builder with default parameters for the level of detail.
        /// </summary>
        public SphereBuilder()
            : this(DefaultSlices, DefaultStacks) { }

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
            int numberOfVertices = (_slices + 1) * (_stacks + 1);
            int numberOfIndices = _slices * _stacks * 6;

            Vector3[] positions = new Vector3[numberOfVertices];
            Vector3[] normals = new Vector3[numberOfVertices];
            Vector2[] texCoords = new Vector2[numberOfVertices];
            uint[] indices = new uint[numberOfIndices];

            float phi;
            float theta;
            float dphi = MathHelper.Pi / _stacks;
            float dtheta = MathHelper.TwoPi / _slices;
            float x, y, z, sc;
            int index = 0;

            for (int stack = 0; stack <= _stacks; ++stack)
            {
                phi = MathHelper.PiOver2 - stack * dphi;
                y = (float)Math.Sin(phi);
                sc = -(float)Math.Cos(phi);

                for (int slice = 0; slice <= _slices; ++slice)
                {
                    theta = slice * dtheta;
                    x = sc * (float)Math.Sin(theta);
                    z = sc * (float)Math.Cos(theta);

                    positions[index] = new Vector3(x, y, z);
                    normals[index] = new Vector3(x, y, z);
                    texCoords[index] = new Vector2((float)slice / (float)_slices, (float)stack / (float)_stacks);
                    
                    index++;
                }
            }

            index = 0;
            int k = _slices + 1;

            for (int stack = 0; stack < _stacks; ++stack)
            {
                for (int slice = 0; slice < _slices; ++slice)
                {
                    indices[index++] = (uint)((stack + 0) * k + slice);
                    indices[index++] = (uint)((stack + 1) * k + slice);
                    indices[index++] = (uint)((stack + 0) * k + slice + 1);

                    indices[index++] = (uint)((stack + 0) * k + slice + 1);
                    indices[index++] = (uint)((stack + 1) * k + slice);
                    indices[index++] = (uint)((stack + 1) * k + slice + 1);
                }
            }

            return new MeshData(positions, normals, texCoords, indices);
        }
    }
}