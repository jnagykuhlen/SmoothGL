﻿using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SmoothGL.Content;
using SmoothGL.Content.Readers;
using SmoothGL.Graphics;
using SmoothGL.Graphics.Geometry;
using SmoothGL.Graphics.Geometry.Builder;
using SmoothGL.Graphics.Shader;
using SmoothGL.Graphics.State;
using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Samples.Windows;

/// <summary>
/// This sample shows how different, advanced rendering techniques can be achieved using SmoothGL.
/// Resources are loaded relying on the content manager, multiple instances of the same geometry are drawn
/// using hardware instancing and a skybox is rendered to demonstrate how cube mapping works. Furthermore,
/// the scene is drawn to a custom frame buffer off-screen as a basis for post-processing effects.
/// </summary>
public class AdvancedTechniquesSampleWindow : SampleWindow
{
    private readonly ContentManager _contentManager;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private VertexBuffer _instanceBuffer;
    private Quad _quad;
    private ShaderProgram _shaderPostProcessing;
    private ShaderProgram _shaderSky;
    private ShaderProgram _shaderTorus;
    private VertexArray _vertexArraySkybox;
    private VertexArray _vertexArrayTorus;

    private FrameBuffer _frameBuffer;
    private ColorTexture2D _frameBufferColorTexture;
    private DepthStencilTexture2D _frameBufferDepthTexture;

    public AdvancedTechniquesSampleWindow()
    {
        // A content manager is required to load resources such as meshes, textures and shaders from disk.
        // We specify that file paths are relative to the "Content" directory which is the location
        // where our content files will be stored. The default content reader for cube textures is
        // overwritten to load images with the provided layout.
        _contentManager = new ContentManager("Content").SetDefaultContentReaders();
        _contentManager.SetContentReader(new TextureCubeReader(TextureFilterMode.Default, CubeTextureLayout.HorizontalCross));
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override void OnLoad()
    {
        // Setup of the skybox. A cube builder is used to create cube geometry in memory. The faces
        // are flipped inwards since the skybox should surround the scene, seen from inside. From
        // this mesh data, a vertex buffer and vertex array is created as a corresponding
        // representation on the GPU that can be drawn later on. Using ContentManager.Add() makes sure
        // that the resources are disposed when unloading the content manager, avoiding manual cleanup.
        var meshDataSkybox = new CubeBuilder().Build().GetFlippedTriangleOrientation();
        var vertexBufferSkybox = _contentManager.Add(meshDataSkybox.ToVertexBuffer(MeshData.VertexPositionSelector, VertexPosition.VertexDeclaration));

        _vertexArraySkybox = _contentManager.Add(new VertexArray(vertexBufferSkybox));

        // Defines the layout of data that is passed to the vertex shader per instance. Since we
        // would like to draw a number of instances of the same model with different transformations,
        // we store a separate world matrix per instance, encoded as four float vectors. From
        // this declaration, a buffer holding information for nine instances is created.
        var instanceDeclaration = new VertexDeclaration(
            new VertexElementFloat(3, 4),
            new VertexElementFloat(4, 4),
            new VertexElementFloat(5, 4),
            new VertexElementFloat(6, 4)
        );

        // Loads a mesh resource stored in the Wavefront OBJ format using the content manager.
        // From the loaded mesh data, a vertex array with corresponding vertex buffer and
        // element buffer is created. Note that the previously created instance buffer is
        // also passed to the vertex array constructor to allow drawing of multiple instances
        // of this vertex array at once.
        var meshDataTorus = _contentManager.Load<MeshData>("Torus.obj");
        var vertexBufferTorus = _contentManager.Add(meshDataTorus.ToVertexBuffer());
        var elementBufferTorus = _contentManager.Add(meshDataTorus.ToElementBuffer());

        _instanceBuffer = _contentManager.Add(new VertexBuffer(9, instanceDeclaration, BufferUsage.Dynamic));
        _vertexArrayTorus = _contentManager.Add(new VertexArray(vertexBufferTorus, _instanceBuffer, elementBufferTorus));

        // Loads different shaders that are required to draw the torus model, the skybox and
        // to apply a post-processing step to the entire screen after rendering the scene.
        // By default, the content manager expects a simple JSON file that lists the paths
        // to the GLSL source code per shader stage.
        _shaderTorus = _contentManager.Load<ShaderProgram>("shader-torus.json");
        _shaderSky = _contentManager.Load<ShaderProgram>("shader-sky.json");
        _shaderPostProcessing = _contentManager.Load<ShaderProgram>("shader-post-processing.json");

        // Sets the values of shader uniforms that do not change during the main loop. Color and
        // cube textures can be assigned to uniform samplers like any other primitive value.
        _shaderPostProcessing.Uniform("NearPlane")?.SetValue(NearPlane);
        _shaderPostProcessing.Uniform("FarPlane")?.SetValue(FarPlane);
        _shaderSky.Uniform("TextureSkybox")?.SetValue(_contentManager.Load<TextureCube>("Skybox.png"));
        _shaderTorus.Uniform("Texture")?.SetValue(_contentManager.Load<ColorTexture2D>("Texture.png"));

        // Creates a quad, which is essentially a vertex array with two triangles spanning the
        // whole screen. This is required for post-processing effects: First, the scene is rendered
        // off-screen and the resulting textures are then drawn to the quad with a post-processing
        // shader, which allows for modifications per pixel.
        _quad = _contentManager.Add(new Quad());

        // Creates a frame buffer, which is used as a target for off-screen rendering. Besides a
        // texture with normal RGBA color channels, a depth-stencil texture is also attached to
        // the frame buffer (from which, however, only the depth component is used). Information
        // on the rendered scene will then be stored in these textures, accessible by the
        // post-processing shader.
        LoadFrameBuffer(ClientSize);
    }

    protected override void OnUnload()
    {
        // Unloads the content manager. This is the only thing required for cleanup, since
        // all created graphics resources were added to the content manager for automatic
        // disposal. Resources directly loaded by the content manager were added implicitly.
        _contentManager.Unload();
        UnloadFrameBuffer();
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

        _shaderSky.Uniform("Projection")?.SetValue(CameraProjection);
        _shaderSky.Uniform("View")?.SetValue(CameraView);
        _shaderSky.Use();
        _vertexArraySkybox.Draw(Primitive.Triangles);

        // Draws multiple instances of the torus object. Depth testing is enabled again
        // to ensure a plausible visual representation of solid geometry. After setting up
        // the shader, a unique world matrix is created for each of the nine instances of
        // the torus we would like to draw. The instance buffer is then updated to contain
        // these matrices. Finally, the vertex array of the torus is drawn multiple times
        // with a single rendering call.
        DepthState.Default.Apply();

        _shaderTorus.Uniform("Projection")?.SetValue(CameraProjection);
        _shaderTorus.Uniform("View")?.SetValue(CameraView);
        _shaderTorus.Use();

        var instanceData = new Matrix4[9];
        for (var i = 0; i < 9; ++i)
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

        _shaderPostProcessing.Uniform("TextureColor")?.SetValue(_frameBufferColorTexture);
        _shaderPostProcessing.Uniform("TextureDepth")?.SetValue(_frameBufferDepthTexture);
        _shaderPostProcessing.Use();
        _quad.Draw();

        // As usual, the back and the front frame buffers need to be swapped to present the
        // result on the display device.
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        _contentManager.UpdateContent();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        UnloadFrameBuffer();
        LoadFrameBuffer(e.Size);
    }

    private void LoadFrameBuffer(Vector2i size)
    {
        _frameBuffer = new FrameBuffer(size.X, size.Y);
        _frameBufferColorTexture = new ColorTexture2D(size.X, size.Y, TextureColorFormat.Rgba32, TextureFilterMode.None);
        _frameBufferDepthTexture = new DepthStencilTexture2D(size.X, size.Y);

        _frameBuffer.Attach(
            _frameBufferDepthTexture.CreateFrameBufferAttachment(),
            _frameBufferColorTexture.CreateFrameBufferAttachment()
        );
    }

    private void UnloadFrameBuffer()
    {
        _frameBufferDepthTexture.Dispose();
        _frameBufferColorTexture.Dispose();
        _frameBuffer.Dispose();
    }
}