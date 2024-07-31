using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Mathematics;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a builder that constructs a unit cylinder aligned along the y-axis with specified level of detail.
    /// </summary>
    public class CylinderBuilder : IGeometryBuilder
    {
        private const int DefaultSlices = 16;

        private int _slices;

        /// <summary>
        /// Creates a new cylinder builder with default parameters for the level of detail.
        /// </summary>
        public CylinderBuilder()
            : this(DefaultSlices) { }

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
            int numberOfVertices = 4 * _slices;
            int numberOfIndices = 6 * (_slices - 2) + 6 * _slices;

            Vector3[] positions = new Vector3[numberOfVertices];
            Vector3[] normals = new Vector3[numberOfVertices];
            uint[] indices = new uint[numberOfIndices];

            float dtheta = MathHelper.TwoPi / _slices;
            int index = 0;

            Vector3[] positionMasks = new Vector3[]
            {
                new Vector3(1, -1, 1),
                new Vector3(1,  1, 1),
                new Vector3(1, -1, 1),
                new Vector3(1,  1, 1)
            };

            Vector3[] normalMasks = new Vector3[]
            {
                new Vector3(0, -1, 0),
                new Vector3(0,  1, 0),
                new Vector3(1,  0, 1),
                new Vector3(1,  0, 1)
            };

            for (int i = 0; i < 4; ++i)
            {
                Vector3 positionMask = positionMasks[i];
                Vector3 normalMask = normalMasks[i];

                for (int slice = 0; slice < _slices; ++slice)
                {
                    float theta = slice * dtheta;
                    float x = (float)Math.Sin(theta);
                    float z = (float)Math.Cos(theta);

                    positions[index] = new Vector3(x, 1.0f, z) * positionMask;
                    normals[index] = new Vector3(x, 1.0f, z) * normalMask;

                    ++index;
                }
            }

            index = 0;

            for (int i = 0; i < 2; ++i)
            {
                int offset = i * _slices;
                
                for (int slice = 2; slice < _slices; ++slice)
                {
                    indices[index++] = (uint)(offset + 0);
                    indices[index++] = (uint)(offset + slice - 1 + i);
                    indices[index++] = (uint)(offset + slice - i);
                }
            }

            for (int slice = 0; slice < _slices; ++slice)
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
}