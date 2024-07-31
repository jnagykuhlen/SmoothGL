using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a quadratic finite plane orthogonal to the z-axis with unit size.
    /// </summary>
    public class Quad : GraphicsResource
    {
        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        
        /// <summary>
        /// Creates a new quad in graphics memory.
        /// </summary>
        public Quad()
        {
            VertexDeclaration declaration = new VertexDeclaration(
                new VertexElementFloat(0, 4)
            );
            Vector4[] data = { new Vector4(1, 1, 0, 1), new Vector4(-1, 1, 0, 1), new Vector4(1, -1, 0, 1), new Vector4(-1, -1, 0, 1) };

            _vertexBuffer = new VertexBuffer(4, declaration, BufferUsage.Static);
            _vertexBuffer.SetData(data);
            _vertexArray = new VertexArray(_vertexBuffer);
        }

        /// <summary>
        /// Draws this quad with the shader program that is currently active.
        /// </summary>
        public void Draw()
        {
            _vertexArray.Draw(Primitive.TriangleStrip);
        }

        protected override void FreeResources()
        {
            _vertexBuffer.Dispose();
            _vertexArray.Dispose();
        }

        protected override string ResourceName
        {
            get
            {
                return "Quad";
            }
        }
    }
}
