using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a custom frame buffer target that allows off-screen rendering. A frame buffer does
/// not provide any storage directly, but a number of buffers or textures can be attached to it.
/// When selected as rendering target, drawn fragments are written to this attached storage instead
/// of being drawn directly to the screen.
/// </summary>
public class FrameBuffer : FrameBufferTarget
{
    private int _frameBufferId;

    /// <summary>
    /// Creates a new frame buffer with specified viewport, determining which pixels are affected
    /// by drawing operations to this frame buffer.
    /// </summary>
    /// <param name="width">Width of the viewport.</param>
    /// <param name="height">Height of the viewport.</param>
    public FrameBuffer(int width, int height)
        : this(new Rectangle(0, 0, width, height))
    {
    }

    /// <summary>
    /// Creates a new frame buffer with specified viewport, determining which pixels are affected
    /// by drawing operations to this frame buffer.
    /// </summary>
    /// <param name="viewport">Viewport area that is affected by drawing operations.</param>
    public FrameBuffer(Rectangle viewport)
        : base(viewport)
    {
        GL.GenFramebuffers(1, out _frameBufferId);
    }

    protected override int Id => _frameBufferId;

    protected override string ResourceName => "FrameBuffer";

    /// <summary>
    /// Attaches storage to this frame buffer. When selected as rendering target, depth and stencil tests
    /// are performed using the specified depth-stencil attachment, whereas color output is written to
    /// the specified color attachments. The order in which color attachments are passed to this function
    /// determines their location in the corresponding shader program.
    /// </summary>
    /// <param name="depthStencilAttachment">
    /// Attachment used for depth and stencil testing.
    /// This parameter can be null in case that depth and stencil testing are not required.
    /// </param>
    /// <param name="colorAttachments">Color attachments to which fragment shader output is written.</param>
    public void Attach(IDepthStencilAttachment depthStencilAttachment, params IColorAttachment[] colorAttachments)
    {
        var maxDrawBuffers = GL.GetInteger(GetPName.MaxDrawBuffers);
        var maxColorAttachments = GL.GetInteger(GetPName.MaxColorAttachments);

        if (colorAttachments.Length > maxDrawBuffers)
            throw new ArgumentException(
                string.Format("The number of color attachments exceeds the limit of {0} draw buffers.", maxDrawBuffers),
                "colorAttachments"
            );

        if (colorAttachments.Length > maxColorAttachments)
            throw new ArgumentException(
                string.Format("The number of color attachments exceeds the limit of {0} attachment points.", maxColorAttachments),
                "colorAttachments"
            );

        var lastTarget = Current;
        Bind();

        if (depthStencilAttachment != null)
            depthStencilAttachment.Attach();

        var drawBuffers = new DrawBuffersEnum[colorAttachments.Length];
        for (var i = 0; i < colorAttachments.Length; ++i)
        {
            colorAttachments[i].Attach(i);
            drawBuffers[i] = DrawBuffersEnum.ColorAttachment0 + i;
        }

        GL.DrawBuffers(drawBuffers.Length, drawBuffers);

        lastTarget.SetAsTarget();
        CheckStatus();
    }

    protected void CheckStatus()
    {
        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            throw new InvalidOperationException("Frame buffer is incomplete.");
    }

    protected override void FreeResources()
    {
        GL.DeleteFramebuffers(1, ref _frameBufferId);
    }
}