using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;


namespace SmoothGL.Graphics
{
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
            float oneOverSqrtTwo = MathHelper.InverseSqrtFast(2.0f);

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-1, 0, -oneOverSqrtTwo),
                new Vector3( 1, 0, -oneOverSqrtTwo),
                new Vector3(0, -1, oneOverSqrtTwo),
                new Vector3(0,  1, oneOverSqrtTwo)
            };

            Vector3[] positions = new Vector3[]
            {
                vertices[2], vertices[1], vertices[3],
                vertices[0], vertices[2], vertices[3],
                vertices[1], vertices[0], vertices[3],
                vertices[0], vertices[1], vertices[2]
            };

            return new MeshData(positions, null);
        }
    }
}
