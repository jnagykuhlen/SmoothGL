using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Defines a vertex by its position.
    /// </summary>
    public struct VertexPosition
    {
        /// <summary>
        /// Gets the corresponding vertex declaration. The position is accessible at location 0
        /// in the vertex shader.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration;

        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public Vector3 Position;

        static VertexPosition()
        {
            VertexDeclaration = new VertexDeclaration(new VertexElementFloat(0, 3));
        }

        /// <summary>
        /// Creates a new vertex with specified position.
        /// </summary>
        /// <param name="position">Position of the vertex.</param>
        public VertexPosition(Vector3 position)
        {
            Position = position;
        }
    }
}
