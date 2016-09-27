using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using SmoothGL.Graphics;

namespace SmoothGL.Samples
{
    /// <summary>
    /// This sample shows the minimal number of steps required to present simple graphics on the screen.
    /// A static, colored triangle is drawn using a custom shader program, a vertex buffer and a vertex array.
    /// </summary>
    public class HelloWorldSample : SampleWindow
    {
        /// <summary>
        /// Vertex shader code for our custom triangle shader. To keep things simple, the string is hardcoded
        /// in this sample. In real applications, shaders are loaded from files using a content manager.
        /// </summary>
        private const string VertexShaderCode = @"
            #version 330
            layout(location = 0) in vec4 vPosition;
            layout(location = 1) in vec3 vColor;
            out vec3 fColor;
            
            void main()
            {
                fColor = vColor;
	            gl_Position = vPosition;
            }
        ";

        /// <summary>
        /// Fragment shader code for our custom triangle shader. Again, the string is hardcoded for simplicity.
        /// </summary>
        private const string FragmentShaderCode = @"
            #version 330
            in vec3 fColor;
            out vec4 color;
            
            void main()
            {
	            color = vec4(fColor, 1.0f);
            }
        ";

        /// <summary>
        /// Defines a custom vertex type which represents the vertices of our triangle. Each of the three vertices
        /// stores a position as homogeneous coordinates and a color intepreted as a vector with red, green and blue channel.
        /// </summary>
        private struct VertexPositionColor
        {
            public Vector4 Position;
            public Vector3 Color;
        }
        
        private ShaderProgram _shaderProgram;
        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;

        public HelloWorldSample()
            : base("Hello World Sample") { }

        protected override void OnLoad(EventArgs e)
        {
            // Creates and compiles a shader program directly from vertex and fragment shader code.
            _shaderProgram = new ShaderProgram(VertexShaderCode, FragmentShaderCode);
            
            // Formal layout declaration of our vertex data represented by the VertexPositionColor struct
            // defined earlier. This declaration communicates to the GPU how the vertex data is organized
            // in memory. In accordance with the struct declaration, we state that the vertex position vector
            // consists of four float values accessible in the shader at location 0, as well as vertex
            // color consisting of three float values bound to location 1.
            VertexDeclaration vertexDeclaration = new VertexDeclaration
            (
                new VertexElementFloat(0, 4),
                new VertexElementFloat(1, 3)
            );

            // Initializes a vertex buffer that can hold three vertices of the previously declared vertex type,
            // which is sufficient to construct a single triangle. The buffer usage is a hint to the driver
            // indicating that we do not intend to change the data in the buffer later on. As vertex buffers
            // cannot be drawn directly, we also create a new vertex array that holds information on how the
            // buffer is drawn. Since we do not specify a corresponding element buffer, each triple of
            // consecutive vertices is interpreted as triangle.
            _vertexBuffer = new VertexBuffer(3, vertexDeclaration, BufferUsage.Static);
            _vertexArray = new VertexArray(_vertexBuffer);

            // Now that the graphics memory for our triangle vertices is allocated, we need to fill it with actual
            // data. For that purpose, an array holding instances of our custom VertexPositionColor struct is sent
            // to the vertex buffer.
            VertexPositionColor[] vertices = new VertexPositionColor[]
            {
                new VertexPositionColor() { Position = new Vector4(-0.6f, -0.6f, 0, 1), Color = new Vector3(1, 0, 0) },
                new VertexPositionColor() { Position = new Vector4( 0.6f, -0.6f, 0, 1), Color = new Vector3(0, 1, 0) },
                new VertexPositionColor() { Position = new Vector4(    0,  0.6f, 0, 1), Color = new Vector3(0, 0, 1) },
            };

            _vertexBuffer.SetData(vertices);
        }

        protected override void OnUnload(EventArgs e)
        {
            // In SmoothGL, it is important that all graphics resources are manually disposed to avoid memory leaks,
            // deallocating graphics memory.
            _shaderProgram.Dispose();
            _vertexArray.Dispose();
            _vertexBuffer.Dispose();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // In each frame, we first clear the screen (represented by the default frame buffer) with a blue
            // background color.
            FrameBufferTarget.Default.Clear(Color.CornflowerBlue);
            
            // We make sure that our custom shader program is used for subsequent rendering operations, in this
            // case for drawing our vertex array. The primitive type is required to indicate that every three
            // vertices in the buffer should form a triangle.
            _shaderProgram.Use();
            _vertexArray.Draw(Primitive.Triangles);

            // Finally, the back and the front frame buffers are swapped to present the drawn frame on the
            // display device. This is the normal procedure for graphics applications using double buffering.
            SwapBuffers();
        }
    }
}
