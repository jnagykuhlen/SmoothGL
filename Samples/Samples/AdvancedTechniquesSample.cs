using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using SmoothGL.Graphics;
using SmoothGL.Content;

namespace SmoothGL.Samples
{
    /// <summary>
    /// This sample shows how different, advanced rendering techniques can be achieved using SmoothGL.
    /// Resources are loaded relying on the content manager, multiple instances of the same geometry are drawn
    /// using hardware instancing and a skybox is rendered to demonstrate how cube mapping works. Furthermore,
    /// the scene is drawn to a custom frame buffer off-screen as a basis for post-processing effects.
    /// </summary>
    public class AdvancedTechniquesSample : SampleWindow
    {
        private ContentManager _contentManager;
        private VertexArray _vertexArrayTorus;
        private VertexArray _vertexArraySkybox;
        private ShaderProgram _shaderTorus;
        private ShaderProgram _shaderSky;
        private ShaderProgram _shaderPostProcessing;
        private VertexBuffer _instanceBuffer;
        private Quad _quad;

        private FrameBuffer _frameBuffer;
        private ColorTexture2D _frameBufferColorTexture;
        private DepthStencilTexture2D _frameBufferDepthTexture;

        public AdvancedTechniquesSample()
            : base("Advanced Techniques Sample")
        {
            // A content manager is required to load resources such as meshes, textures and shaders from disk.
            // We specify that file paths are relative to the "Content" directory which is the location
            // where our content files will be stored. The default content reader for cube textures is
            // overwritten to load images with the provided layout.
            _contentManager = ContentManager.CreateDefault("Content");
            _contentManager.SetContentReader<TextureCube>(new TextureCubeReader(TextureFilterMode.Default, false, CubeTextureLayout.HorizontalCross));
        }

        protected override void OnLoad(EventArgs e)
        {
            // Setup of the skybox. A cube builder is used to create cube geometry in memory. The faces
            // are flipped inwards since the skybox should surround the scene, seen from inside. From
            // this mesh data, a vertex buffer and vertex array is created as a corresponding
            // representation on the GPU that can be drawn later on.
            MeshData meshDataSkybox = (new CubeBuilder()).Build().GetFlippedTriangleOrientation();
            VertexBuffer vertexBufferSkybox = meshDataSkybox.ToVertexBuffer(MeshData.VertexPositionSelector, VertexPosition.VertexDeclaration);

            _vertexArraySkybox = new VertexArray(vertexBufferSkybox);

            // Adds the created graphics resources to the content manager. This makes sure that the
            // resources are disposed when unloading the content manager, avoiding manual cleanup.
            _contentManager.Add(vertexBufferSkybox);
            _contentManager.Add(_vertexArraySkybox);

            // Defines the layout of data that is passed to the vertex shader per instance. Since we
            // would like to draw a number of instances of the same model with different transformations,
            // we store a separate world matrix per instance, encoded as four float vectors. From
            // this declaration, a buffer holding information for nine instances is created.
            VertexDeclaration instanceDeclaration = new VertexDeclaration(
                new VertexElementFloat(3, 4),
                new VertexElementFloat(4, 4),
                new VertexElementFloat(5, 4),
                new VertexElementFloat(6, 4)
            );

            _instanceBuffer = new VertexBuffer(9, instanceDeclaration, BufferUsage.Dynamic);

            // Loads a mesh resource stored in the Wavefront OBJ format using the content manager.
            // From the loaded mesh data, a vertex array with corresponding vertex buffer and
            // element buffer is created. Note that the previously created instance buffer is
            // also passed to the vertex array constructor to allow drawing of multiple instances
            // of this vertex array at once.
            MeshData meshDataTorus = _contentManager.Load<MeshData>("Torus.obj");
            VertexBuffer vertexBufferTorus = meshDataTorus.ToVertexBuffer();
            ElementBuffer elementBufferTorus = meshDataTorus.ToElementBuffer();

            _vertexArrayTorus = new VertexArray(vertexBufferTorus, _instanceBuffer, elementBufferTorus);

            // Again, the graphics resources are added to the content manager for automatic disposal.
            _contentManager.Add(vertexBufferTorus);
            _contentManager.Add(elementBufferTorus);
            _contentManager.Add(_vertexArrayTorus);
            _contentManager.Add(_instanceBuffer);
            
            // Loads different shaders that are required to draw the torus model, the skybox and
            // to apply a post-processing step to the entire screen after rendering the scene.
            // By default, the content manager expects a simple XML file that lists the paths
            // to the GLSL source code per shader stage.
            _shaderTorus = _contentManager.Load<ShaderProgram>("ShaderTorus.xml");
            _shaderSky = _contentManager.Load<ShaderProgram>("ShaderSky.xml");
            _shaderPostProcessing = _contentManager.Load<ShaderProgram>("ShaderPostProcessing.xml");

            // Sets the values of shader uniforms that do not change during the main loop. Color and
            // cube textures can be assigned to uniform samplers like any other primitive value.
            _shaderPostProcessing.Uniform("NearPlane").Value = NearPlane;
            _shaderPostProcessing.Uniform("FarPlane").Value = FarPlane;
            _shaderSky.Uniform("TextureSkybox").Value = _contentManager.Load<TextureCube>("Skybox.png");
            _shaderTorus.Uniform("Texture").Value = _contentManager.Load<ColorTexture2D>("Texture.png");

            // Creates a quad, which is essentially a vertex array with two triangles spanning the
            // whole screen. This is required for post-processing effects: First, the scene is rendered
            // off-screen and the resulting textures are then drawn to the quad with a post-processing
            // shader, which allows for modifications per pixel.
            _quad = new Quad();
            _contentManager.Add(_quad);

            // Creates a frame buffer, which is used as a target for off-screen rendering. Besides a
            // texture with normal RGBA color channels, a depth-stencil texture is also attached to
            // the frame buffer (from which, however, only the depth component is used). Information
            // on the rendered scene will then be stored in these textures, accessible by the
            // post-processing shader.
            _frameBuffer = new FrameBuffer(Width, Height);
            _frameBufferColorTexture = new ColorTexture2D(Width, Height, TextureColorFormat.Rgba32, TextureFilterMode.None);
            _frameBufferDepthTexture = new DepthStencilTexture2D(Width, Height);

            _frameBuffer.Attach(
                _frameBufferDepthTexture.CreateFrameBufferAttachment(),
                _frameBufferColorTexture.CreateFrameBufferAttachment()
            );

            _contentManager.Add(_frameBuffer);
            _contentManager.Add(_frameBufferColorTexture);
            _contentManager.Add(_frameBufferDepthTexture);
        }

        protected override void OnUnload(EventArgs e)
        {
            // Unloads the content manager. This is the only thing required for cleanup, since
            // all created graphics resources were added to the content manager for automatic
            // disposal. Resources directly loaded by the content manager were added implicitly.
            _contentManager.Unload();
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Targets the custom frame buffer for off-screen rendering. All subsequent draw
            // operations do not affect the screen directly, but are instead forwarded to the
            // textures attached to this frame buffer. As normal, the old data is cleared
            // before starting to render the scene.
            _frameBuffer.SetAsTarget();
            _frameBuffer.Clear(TargetOptions.All, Color.CornflowerBlue, 1.0f, 0);
            
            // Draws the skybox. First, depth testing is disabled to prevent the skybox from
            // appearing in front of any object drawn later on. The corresponding shader is
            // then supplied with recent values for the camera view and projection before
            // being used to draw the skybox geometry.
            DepthState.None.Apply();
            
            _shaderSky.Uniform("Projection").Value = CameraProjection;
            _shaderSky.Uniform("View").Value = CameraView;
            _shaderSky.Use();
            _vertexArraySkybox.Draw(Primitive.Triangles);

            // Draws multiple instances of the torus object. Depth testing is enabled again
            // to ensure a plausible visual representation of solid geometry. After setting up
            // the shader, a unique world matrix is created for each of the nine instances of
            // the torus we would like to draw. The instance buffer is then updated to contain
            // these matrices. Finally, the vertex array of the torus is drawn multiple times
            // with a single rendering call.
            DepthState.Default.Apply();
            
            _shaderTorus.Uniform("Projection").Value = CameraProjection;
            _shaderTorus.Uniform("View").Value = CameraView;
            _shaderTorus.Use();

            Matrix4[] instanceData = new Matrix4[9];
            for (int i = 0; i < 9; ++i)
            {
                instanceData[i] = Matrix4.CreateScale(0.6f) *
                                  Matrix4.CreateRotationY(2.3f * ElapsedTime) *
                                  Matrix4.CreateRotationX(0.8f * ElapsedTime) *
                                  Matrix4.CreateTranslation(1.4f * Vector3.UnitX * (i - 4));
            }

            _instanceBuffer.SetData(instanceData);
            _vertexArrayTorus.DrawMultiple(Primitive.Triangles, 9);

            // Since we targeted the custom frame buffer when drawing the scene, nothing has
            // been displayed on the screen yet. In this final step, the post-processing
            // shader is provided access to the textures which hold color and depth data
            // of the previously rendered scene. Using this shader, the quad is drawn to
            // cover every pixel of the screen and therefore allowing to modify every pixel
            // arbitrarily in the fragment shader.
            FrameBufferTarget.Default.SetAsTarget();
            FrameBufferTarget.Default.Clear(TargetOptions.All, Color.CornflowerBlue, 1.0f, 0);

            _shaderPostProcessing.Uniform("TextureColor").Value = _frameBufferColorTexture;
            _shaderPostProcessing.Uniform("TextureDepth").Value = _frameBufferDepthTexture;
            _shaderPostProcessing.Use();
            _quad.Draw();

            // As usual, the back and the front frame buffers need to be swapped to present the
            // result on the display device.
            SwapBuffers();
        }
    }
}
