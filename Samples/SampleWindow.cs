using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Samples;

/// <summary>
/// Base class for all samples that initializes the graphics context and implements
/// some basic camera control.
/// </summary>
public abstract class SampleWindow : GameWindow
{
    protected const float FarPlane = 20.0f;
    protected const float NearPlane = 0.1f;
    
    private float _cameraDistance;
    private float _cameraPitch;
    private float _cameraYaw;
    private bool _drag;

    /// <summary>
    /// Creates a new sample window with fixed size and specified title. A graphics context is
    /// initialized for OpenGL version 3.3.
    /// </summary>
    protected SampleWindow()
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        ElapsedTime = 0.0f;
        _cameraPitch = 0.0f;
        _cameraYaw = 0.0f;
        _cameraDistance = 5.0f;
        _drag = false;

        UpdateProjection();
    }

    /// <summary>
    /// Gets the camera's projection matrix.
    /// </summary>
    protected Matrix4 CameraProjection { get; private set; }

    /// <summary>
    /// Gets the camera's view matrix. The camera angle and zoom level can be adjusted with mouse controls.
    /// </summary>
    protected Matrix4 CameraView => Matrix4.CreateRotationY(_cameraYaw) *
                                    Matrix4.CreateRotationX(_cameraPitch) *
                                    Matrix4.CreateTranslation(0, 0, -_cameraDistance);

    /// <summary>
    /// Gets the time since window initialization in seconds, useful for time-based animations.
    /// </summary>
    protected float ElapsedTime { get; private set; }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        if (e.Key == Keys.Escape)
            Close();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left)
            _drag = true;
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left)
            _drag = false;
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        if (_drag)
        {
            _cameraPitch += 0.01f * e.DeltaY;
            _cameraYaw += 0.01f * e.DeltaX;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        _cameraDistance = MathHelper.Clamp(_cameraDistance - 0.2f * e.OffsetY, 1.0f, 10.0f);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        ElapsedTime += (float)e.Time;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        UpdateProjection();
        UpdateViewport();
    }

    /// <summary>
    /// Re-calculates the camera's projection matrix, which is required when the aspect ratio of the
    /// window changes.
    /// </summary>
    private void UpdateProjection()
    {
        var aspectRatio = (float)ClientSize.X / ClientSize.Y;
        CameraProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), aspectRatio, NearPlane, FarPlane);
    }

    /// <summary>
    /// Sets the size of the default frame buffer to the size of the window, which is required when
    /// the window is resized.
    /// </summary>
    private void UpdateViewport()
    {
        FrameBufferTarget.Default.Viewport = new Rectangle(0, 0, ClientSize.X, ClientSize.Y);
    }
}