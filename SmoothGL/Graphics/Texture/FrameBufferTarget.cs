using System.Drawing;
using OpenTK.Graphics.OpenGL;
using SmoothGL.Graphics.Internal;

namespace SmoothGL.Graphics;

/// <summary>
///     Represents an abstract target for draw operations, i.e., an object
///     that fragment shader output is written to.
/// </summary>
public abstract class FrameBufferTarget : GraphicsResource
{
    private static int _currentFrameBufferId;
    private static FrameBufferTarget _defaultFrameBuffer;
    private static FrameBufferTarget _currentFrameBuffer;

    private Rectangle _viewport;

    protected FrameBufferTarget(Rectangle viewport)
    {
        _viewport = viewport;
    }

    /// <summary>
    ///     Gets the default frame buffer. When selected as target, all
    ///     draw operations directly affect the pixels on the screen.
    /// </summary>
    public static FrameBufferTarget Default
    {
        get
        {
            if (_defaultFrameBuffer == null)
            {
                _defaultFrameBuffer = new DefaultFrameBuffer();
                if (_currentFrameBuffer == null)
                    _currentFrameBuffer = _defaultFrameBuffer;
            }

            return _defaultFrameBuffer;
        }
    }

    /// <summary>
    ///     Gets the frame buffer that is currently selected as target, i.e., the frame
    ///     buffer that subsequent drawing operations will be performed on.
    /// </summary>
    public static FrameBufferTarget Current
    {
        get
        {
            if (_currentFrameBuffer == null)
                _currentFrameBuffer = Default;
            return _currentFrameBuffer;
        }
    }

    /// <summary>
    ///     Gets or sets the viewport for this frame buffer target, determining which
    ///     pixels are affected by drawing operations to this frame buffer target.
    /// </summary>
    public Rectangle Viewport
    {
        get => _viewport;
        set
        {
            _viewport = value;
            if (IsTarget)
                ApplyViewport();
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this frame buffer is currently selected as
    ///     target.
    /// </summary>
    public bool IsTarget => _currentFrameBuffer == this;

    protected abstract int Id { get; }

    /// <summary>
    ///     Clears this frame buffer target with the specified color. If present, the depth and stencil
    ///     attachments are overwritten with maximum depth and zero, respectively.
    /// </summary>
    /// <param name="color">The color written to all color attachments.</param>
    public void Clear(Color color)
    {
        Clear(TargetOptions.All, color, 1.0f, 0);
    }

    /// <summary>
    ///     Clears this frame buffer target, replacing all values in the specified attachments by the
    ///     provided clearing values.
    /// </summary>
    /// <param name="options">Specifies which attachments are affected by the clearing operation.</param>
    /// <param name="color">The color written to all color attachments.</param>
    /// <param name="depth">The depth value written to the depth attachment, if present.</param>
    /// <param name="stencil">The stencil value written to the stencil attachment, if present.</param>
    public void Clear(TargetOptions options, Color color, float depth, int stencil)
    {
        if (!IsTarget)
            throw new InvalidOperationException("The frame buffer needs to be set as target before clearing.");

        GL.ClearColor(color);
        GL.ClearDepth(depth);
        GL.ClearStencil(stencil);
        GL.Clear((ClearBufferMask)options);
    }

    public void CopyFrom(FrameBufferTarget source, TargetOptions options)
    {
        CopyFrom(source, options, source.Viewport, Viewport);
    }

    /// <summary>
    ///     Copies data from another frame buffer target to this frame buffer target, affecting the specified attachments and
    ///     areas. The copy operation requires this frame buffer to be selected as target.
    /// </summary>
    /// <param name="source">The frame buffer target from which data is copied.</param>
    /// <param name="options">Specifies which attachments are affected by the copy operation.</param>
    /// <param name="sourceRectangle">Area in the source frame buffer target from which data is copied.</param>
    /// <param name="destinationRectangle">Area in this frame buffer target to which data is copied.</param>
    public void CopyFrom(FrameBufferTarget source, TargetOptions options, Rectangle sourceRectangle, Rectangle destinationRectangle)
    {
        if (!IsTarget)
            throw new InvalidOperationException("The frame buffer needs to be set as target before copying data.");

        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, source.Id);
        GL.BlitFramebuffer(
            sourceRectangle.Left, sourceRectangle.Top, sourceRectangle.Right, sourceRectangle.Bottom,
            destinationRectangle.Left, destinationRectangle.Top, destinationRectangle.Right, destinationRectangle.Bottom,
            (ClearBufferMask)options,
            BlitFramebufferFilter.Nearest
        );
    }

    protected void Bind()
    {
        if (_currentFrameBufferId != Id)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, Id);
            _currentFrameBufferId = Id;
        }
    }

    protected void ApplyViewport()
    {
        GL.Viewport(_viewport);
    }

    /// <summary>
    ///     Sets this frame buffer as target. As a result, subsequent draw operations
    ///     write to this frame buffer target.
    /// </summary>
    public void SetAsTarget()
    {
        if (_currentFrameBuffer != this)
        {
            Bind();
            ApplyViewport();
            _currentFrameBuffer = this;
        }
    }
}