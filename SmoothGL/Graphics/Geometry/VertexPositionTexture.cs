using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Defines a vertex by its position and texture coordinate.
    /// </summary>
    public struct VertexPositionTexture
    {
        /// <summary>
        /// Gets the corresponding vertex declaration. The position is accessible at location 0
        /// and the texture coordinate at location 1 in the vertex shader.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration;

        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The texture coordinate of the vertex.
        /// </summary>
        public Vector2 TextureCoordinate;

        static VertexPositionTexture()
        {
            VertexDeclaration = new VertexDeclaration(
                new VertexElementFloat(0, 3),
                new VertexElementFloat(1, 2)
            );
        }

        /// <summary>
        /// Creates a new vertex with specified position and texture coordinate.
        /// </summary>
        /// <param name="position">Position of the vertex.</param>
        /// <param name="textureCoordinate">Texture coordinate of the vertex.</param>
        public VertexPositionTexture(Vector3 position, Vector2 textureCoordinate)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
        }
    }
}
