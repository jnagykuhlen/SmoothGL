using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a buffer persistent in graphics memory, storing a 24-bit depth value and an 8-bit stencil value
/// per pixel. Single depth-stencil buffers can be bound to frame buffers to allow depth and stencil testing,
/// but the content of this buffer cannot be accessed explicitly. In case that depth or stencil values need to be
/// read outside the context of depth and stencil testing, consider using the <see cref="DepthStencilTexture2D" />
/// class instead.
/// </summary>
public class DepthStencilBuffer : GraphicsResource
{
    private int _renderBufferId;

    /// <summary>
    /// Creates a new depth-stencil buffer.
    /// </summary>
    /// <param name="width">Width of the buffer in pixels.</param>
    /// <param name="height">Height of the buffer in pixels.</param>
    public DepthStencilBuffer(int width, int height)
    {
        GL.GenRenderbuffers(1, out _renderBufferId);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderBufferId);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
    }

    protected override string ResourceName => "DepthStencilBuffer";

    /// <summary>
    /// Creates a new frame buffer attachment that can be used to attach this buffer to a frame buffer
    /// by calling its <see cref="FrameBuffer.Attach(IDepthStencilAttachment, IColorAttachment[])" /> method.
    /// This allows for depth and stencil testing when the frame buffer is set as target.
    /// </summary>
    /// <returns>Frame buffer attachment.</returns>
    public IDepthStencilAttachment CreateFrameBufferAttachment()
    {
        return new Attachment(_renderBufferId);
    }

    protected override void FreeResources()
    {
        GL.DeleteRenderbuffers(1, ref _renderBufferId);
    }

    private class Attachment : IDepthStencilAttachment
    {
        private readonly int _id;

        public Attachment(int id)
        {
            _id = id;
        }

        public void Attach()
        {
            GL.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer,
                _id
            );
        }
    }
}