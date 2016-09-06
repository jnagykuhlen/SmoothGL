using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a buffer storing vertex data, persistent in graphics memory.
    /// </summary>
    public class VertexBuffer : Buffer
    {
        private int _numberOfVertices;
        private VertexDeclaration _vertexDeclaration;

        /// <summary>
        /// Creates a new vertex buffer.
        /// </summary>
        /// <param name="numberOfVertices">Maximum number of vertices stored in this buffer.</param>
        /// <param name="vertexDeclaration">Vertex declaration defining how the buffer data is interpreted.</param>
        /// <param name="usage">Hint for the driver concerning the frequency the data in this buffer is expected to change.</param>
        public VertexBuffer(int numberOfVertices, VertexDeclaration vertexDeclaration, BufferUsage usage)
            : base(numberOfVertices * vertexDeclaration.StrideSize, BufferTarget.ArrayBuffer, usage)
        {
            _numberOfVertices = numberOfVertices;
            _vertexDeclaration = vertexDeclaration;
        }

        /// <summary>
        /// Resizes this buffer in a way that it can store the specified number of vertices. All vertex data in this buffer will be discarded.
        /// </summary>
        /// <param name="numberOfVertices">Maximum number of vertices stored in this buffer.</param>
        public new void Resize(int numberOfVertices)
        {
            _numberOfVertices = numberOfVertices;
            base.Resize(numberOfVertices * _vertexDeclaration.StrideSize);
        }

        /// <summary>
        /// Uploads vertex data to the GPU.
        /// </summary>
        /// <typeparam name="T">Vertex type.</typeparam>
        /// <param name="data">Vertex data to store in this buffer.</param>
        public void SetData<T>(T[] data) where T : struct
        {
            if (data.Length > _numberOfVertices)
                throw new ArgumentException("Cannot set data that exceeds buffer size.");

            SetData(data, 0, data.Length * _vertexDeclaration.StrideSize);
        }

        protected override string ResourceName
        {
            get
            {
                return "VertexBuffer";
            }
        }

        /// <summary>
        /// Gets the maximum number of vertices which can be stored in this buffer.
        /// </summary>
        public int NumberOfVertices
        {
            get
            {
                return _numberOfVertices;
            }
        }

        /// <summary>
        /// Gets the associated vertex declaration.
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }
}
