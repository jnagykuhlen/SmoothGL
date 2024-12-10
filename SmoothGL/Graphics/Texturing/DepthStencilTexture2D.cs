using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Texturing;

/// <summary>
/// Represents a two-dimensional texture persistent in graphics memory, storing a 24-bit depth value and an 8-bit
/// stencil value per pixel. Single depth-stencil textures can be bound to frame buffers to allow depth and stencil
/// testing. Unlike the <see cref="DepthStencilBuffer" />, this texture can be assigned to a shader uniform when not
/// attached to an active frame buffer, allowing direct access to stored depth and stencil values.
/// </summary>
public class DepthStencilTexture2D : Texture
{
    /// <summary>
    /// Creates a new depth-stencil texture.
    /// </summary>
    /// <param name="width">Texture width.</param>
    /// <param name="height">Texture height.</param>
    public DepthStencilTexture2D(int width, int height)
        : base(TextureTarget.Texture2D, TextureFilterMode.None)
    {
        Width = width;
        Height = height;
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, width, height, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, IntPtr.Zero);
    }

    /// <summary>
    /// Gets the width of this texture in pixels.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Gets the height of this texture in pixels.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Creates a new frame buffer attachment that can be used to attach this texture to a frame buffer
    /// by calling its <see cref="FrameBuffer.Attach(IDepthStencilAttachment, IColorAttachment[])" /> method.
    /// This allows for depth and stencil testing when the frame buffer is set as target.
    /// </summary>
    /// <returns>Frame buffer attachment.</returns>
    public IDepthStencilAttachment CreateFrameBufferAttachment() => new Attachment(Id);

    private class Attachment(int id) : IDepthStencilAttachment
    {
        public void Attach()
        {
            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthStencilAttachment,
                TextureTarget.Texture2D,
                id,
                0
            );
        }
    }
}