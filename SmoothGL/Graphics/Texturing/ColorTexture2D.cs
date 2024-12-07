using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SmoothGL.Content;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;


namespace SmoothGL.Graphics.Texturing;

/// <summary>
/// Defines a two-dimensional texture persistent in graphics memory, storing a grid of color values.
/// </summary>
public class ColorTexture2D : Texture2D, IHotSwappable<ColorTexture2D>
{
    /// <summary>
    /// Creates a new color texture with RGBA format and default filter mode.
    /// </summary>
    /// <param name="width">Texture width.</param>
    /// <param name="height">Texture height.</param>
    public ColorTexture2D(int width, int height)
        : this(width, height, TextureColorFormat.Rgba32, TextureFilterMode.Default)
    {
    }

    /// <summary>
    /// Creates a new color texture with specified color format and filtering.
    /// </summary>
    /// <param name="width">Texture width.</param>
    /// <param name="height">Texture height.</param>
    /// <param name="format">Specifies the format of each color value in memory.</param>
    /// <param name="filterMode">Specifies the filtering of the texture.</param>
    public ColorTexture2D(int width, int height, TextureColorFormat format, TextureFilterMode filterMode)
        : base(width, height, (PixelInternalFormat)format, PixelFormat.Rgba, PixelType.Float, filterMode)
    {
        Format = format;
    }

    /// <summary>
    /// Gets the format of each color value in memory.
    /// </summary>
    public TextureColorFormat Format { get; private set; }

    protected override string ResourceName => "ColorTexture";

    /// <summary>
    /// Stores color data in this texture. The provided data array must have exactly width * height elements.
    /// </summary>
    /// <param name="data">Color data to store in the texture.</param>
    public void SetData(Color4[] data)
    {
        if (data.Length != Width * Height)
            throw new ArgumentException("The provided texture data does not contain the required number of color values.");

        Bind();
        GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)Format, Width, Height, 0, PixelFormat.Rgba, PixelType.Float, data);
        UpdateMipmaps();
    }

    /// <summary>
    /// Stores image data in this texture. The image must have the same size as this texture.
    /// </summary>
    /// <param name="imageData">Image data to store in the texture.</param>
    public void SetImageData(ImageData imageData)
    {
        if (imageData.Width != Width || imageData.Height != Height)
            throw new ArgumentException("The size of the provided image data does not match the size of this texture.");

        Bind();
        GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)Format, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData.Data);
        UpdateMipmaps();
    }

    /// <summary>
    /// Reads the color data stored in this texture back into client memory.
    /// </summary>
    /// <returns>Color data.</returns>
    public Color4[] GetData()
    {
        Bind();
        var data = new Color4[Width * Height];
        GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.Float, data);
        return data;
    }

    /// <summary>
    /// Creates image data from the color data stored in this texture.
    /// </summary>
    /// <returns>New image data with color data from this texture.</returns>
    public ImageData GetImageData()
    {
        var data = new byte[4 * Width * Height];
        
        Bind();
        GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

        return new ImageData(Width, Height, data);
    }

    /// <summary>
    /// Creates a new frame buffer attachment that can be used to attach this texture to a frame buffer
    /// by calling its <see cref="FrameBuffer.Attach(IDepthStencilAttachment, IColorAttachment[])" /> method.
    /// </summary>
    /// <returns>Frame buffer attachment.</returns>
    public IColorAttachment CreateFrameBufferAttachment() => new Attachment(Id);

    private class Attachment(int id) : IColorAttachment
    {
        public void Attach(int index)
        {
            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0 + index,
                TextureTarget.Texture2D,
                id,
                0
            );
        }
    }

    void IHotSwappable<ColorTexture2D>.HotSwap(ColorTexture2D other)
    {
        base.HotSwap(other);
        Format = other.Format;
    }
}