using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

using SmoothGL.Graphics;

namespace SmoothGL.Samples
{
    /// <summary>
    /// Base class for all samples that initializes the graphics context and implements
    /// some basic camera control.
    /// </summary>
    public class SampleWindow : GameWindow
    {
        private float NearPlane = 0.1f;
        private float FarPlane = 20.0f;
        
        private Matrix4 _projection;
        private float _elapsedTime;
        private float _cameraPitch;
        private float _cameraYaw;
        private float _cameraDistance;
        private bool _drag;

        /// <summary>
        /// Creates a new sample window with fixed size and specified title. A graphics context is
        /// initialized for OpenGL version 3.3.
        /// </summary>
        /// <param name="title"></param>
        public SampleWindow(string title)
           : base(1024, 768, GraphicsMode.Default, title, GameWindowFlags.Default, DisplayDevice.Default, 3, 3, GraphicsContextFlags.Default)
        {
            _elapsedTime = 0.0f;
            _cameraPitch = 0.0f;
            _cameraYaw = 0.0f;
            _cameraDistance = 5.0f;
            _drag = false;

            UpdateProjection();
        }
        
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Exit();
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
                _cameraPitch += 0.01f * e.YDelta;
                _cameraYaw += 0.01f * e.XDelta;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _cameraDistance = MathHelper.Clamp(_cameraDistance - 0.2f * e.Delta, 1.0f, 10.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _elapsedTime += (float)e.Time;
        }
        
        protected override void OnResize(EventArgs e)
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
            float aspectRatio = (float)Width / (float)Height;
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), aspectRatio, NearPlane, FarPlane);
        }

        /// <summary>
        /// Sets the size of the default frame buffer to the size of the window, which is required when
        /// the window is resized.
        /// </summary>
        private void UpdateViewport()
        {
            FrameBufferTarget.Default.Viewport = new Rectangle(0, 0, Width, Height);
        }

        /// <summary>
        /// Gets the camera's projection matrix.
        /// </summary>
        protected Matrix4 CameraProjection
        {
            get
            {
                return _projection;
            }
        }

        /// <summary>
        /// Gets the camera's view matrix. The camera angle and zoom can be adjusted with mouse controls.
        /// </summary>
        protected Matrix4 CameraView
        {
            get
            {
                return Matrix4.CreateRotationY(_cameraYaw) * Matrix4.CreateRotationX(_cameraPitch) * Matrix4.CreateTranslation(0, 0, -_cameraDistance);
            }
        }

        /// <summary>
        /// Gets the time since window initialization in seconds, useful for time-based animations.
        /// </summary>
        protected float ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
        }
    }
}
