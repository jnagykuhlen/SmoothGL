using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;


namespace SmoothGL.Graphics.Texturing;

/// <summary>
/// Defines a cube texture persistent in graphics memory, storing a separate grid of color values for each of the six
/// faces of a cube.
/// </summary>
public class TextureCube : Texture
{
    private const int AllFacesBitMask = 0b111111;

    private int _setFacesBitMask;

    /// <summary>
    /// Creates a new cube texture with RGBA format and default filter mode.
    /// </summary>
    /// <param name="width">Cube face width in pixels.</param>
    /// <param name="height">Cube face height in pixels.</param>
    public TextureCube(int width, int height)
        : this(width, height, TextureColorFormat.Rgba32, TextureFilterMode.Default)
    {
    }

    /// <summary>
    /// Creates a new cube texture with specified color format and filtering.
    /// </summary>
    /// <param name="width">Cube face width in pixels.</param>
    /// <param name="height">Cube face height in pixels.</param>
    /// <param name="format">Specifies the format of each color value in memory.</param>
    /// <param name="filterMode">Specifies the filtering of the cube face textures.</param>
    public TextureCube(int width, int height, TextureColorFormat format, TextureFilterMode filterMode)
        : base(TextureTarget.TextureCubeMap, filterMode)
    {
        Width = width;
        Height = height;
        Format = format;

        GL.Enable(EnableCap.TextureCubeMapSeamless);
        for (var i = 0; i < 6; ++i)
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, (PixelInternalFormat)format, width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
    }

    protected override string ResourceName => "TextureCube";

    /// <summary>
    /// Gets the width of each cube face in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of each cube face in pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the format of each color value in memory.
    /// </summary>
    public TextureColorFormat Format { get; }

    /// <summary>
    /// Stores color data in the specified face of this texture. The provided data array must have exactly width * height
    /// elements.
    /// </summary>
    /// <param name="data">Color data to store in the specified face.</param>
    /// <param name="cubeFace">The face to store color data in.</param>
    public void SetData(Color4[] data, TextureCubeFace cubeFace)
    {
        if (data.Length != Width * Height)
            throw new ArgumentException("The provided texture data does not contain the required number of color values.");

        Bind();
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + (int)cubeFace, 0, (PixelInternalFormat)Format, Width, Height, 0, PixelFormat.Rgba, PixelType.Float, data);
        TryUpdateMipmaps(cubeFace);
    }

    /// <summary>
    /// Stores image data in the specified face of this texture. The image size must match the face size of this
    /// texture.
    /// </summary>
    /// <param name="imageData">Image data to store in the specified face.</param>
    /// <param name="cubeFace">The face to store image data in.</param>
    public void SetData(ImageData imageData, TextureCubeFace cubeFace)
    {
        if (imageData.Width != Width || imageData.Height != Height)
            throw new ArgumentException("The size of the provided image data does not match the size of this texture.");

        Bind();
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + (int)cubeFace, 0, (PixelInternalFormat)Format, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData.Data);
        TryUpdateMipmaps(cubeFace);
    }

    private void TryUpdateMipmaps(TextureCubeFace textureCubeFace)
    {
        _setFacesBitMask |= 1 << (int)textureCubeFace;
        if (_setFacesBitMask == AllFacesBitMask)
            UpdateMipmaps();
    }

    /// <summary>
    /// Reads the color data stored in the specified face back into client memory.
    /// </summary>
    /// <param name="cubeFace">The face to read color data from.</param>
    /// <returns>Color data.</returns>
    public Color4[] GetData(TextureCubeFace cubeFace)
    {
        Bind();
        var data = new Color4[Width * Height];
        GL.GetTexImage(TextureTarget.TextureCubeMapPositiveX + (int)cubeFace, 0, PixelFormat.Rgba, PixelType.Float, data);
        return data;
    }

    /// <summary>
    /// Creates image data from the color data stored in the specified face of this texture.
    /// </summary>
    /// <returns>New image data with color data from this texture.</returns>
    public ImageData GetImageData(TextureCubeFace cubeFace)
    {
        var data = new byte[4 * Width * Height];

        Bind();
        GL.GetTexImage(TextureTarget.TextureCubeMapPositiveX + (int)cubeFace, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

        return new ImageData(Width, Height, data);
    }

    /// <summary>
    /// Creates a new frame buffer attachment that can be used to attach a specified face of this texture to a
    /// frame buffer by calling its <see cref="FrameBuffer.Attach(IDepthStencilAttachment, IColorAttachment[])" /> method.
    /// </summary>
    /// <param name="cubeFace">The face to create a frame buffer attachment for.</param>
    /// <returns>Frame buffer attachment.</returns>
    public IColorAttachment CreateFrameBufferAttachment(TextureCubeFace cubeFace) => new Attachment(Id, cubeFace);

    private class Attachment(int id, TextureCubeFace cubeFace) : IColorAttachment
    {
        public void Attach(int index)
        {
            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0 + index,
                TextureTarget.TextureCubeMapPositiveX + (int)cubeFace,
                id,
                0
            );
        }
    }
}