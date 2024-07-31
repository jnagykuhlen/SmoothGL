using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SmoothGL.Graphics;

/// <summary>
///     Defines a one-dimensional texture persistent in graphics memory, storing an array of color values.
/// </summary>
public class Texture1D : Texture
{
    /// <summary>
    ///     Creates a new one-dimensional color texture with RGBA format and default filter mode.
    /// </summary>
    /// <param name="width">Texture width.</param>
    public Texture1D(int width)
        : this(width, TextureColorFormat.Rgba32, TextureFilterMode.Default)
    {
    }

    /// <summary>
    ///     Creates a new one-dimensional color texture with specified color format and filtering.
    /// </summary>
    /// <param name="width">Texture width.</param>
    /// <param name="format">Specifies the format of each color value in memory.</param>
    /// <param name="filterMode">Specifies the filtering of the texture.</param>
    public Texture1D(int width, TextureColorFormat format, TextureFilterMode filterMode)
        : base(TextureTarget.Texture1D, filterMode)
    {
        Width = width;
        Format = format;
        GL.TexImage1D(TextureTarget.Texture1D, 0, (PixelInternalFormat)format, width, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
    }

    /// <summary>
    ///     Gets the width of this texture.
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     Gets the format of each color value in memory.
    /// </summary>
    public TextureColorFormat Format { get; }

    /// <summary>
    ///     Stores color data in this texture. The length of the provided data array must match the width of this texture.
    /// </summary>
    /// <param name="data">Color data to store in the texture.</param>
    public void SetData(Color4[] data)
    {
        if (data.Length != Width)
            throw new ArgumentException("The provided texture data does not contain the required number of color values.");

        Bind();
        GL.TexImage1D(TextureTarget.Texture1D, 0, (PixelInternalFormat)Format, Width, 0, PixelFormat.Rgba, PixelType.Float, data);
        UpdateMipmaps();
    }

    /// <summary>
    ///     Reads the color data stored in this texture back into client memory.
    /// </summary>
    /// <returns>Color data.</returns>
    public Color4[] GetData()
    {
        Bind();
        var data = new Color4[Width];
        GL.GetTexImage(TextureTarget.Texture1D, 0, PixelFormat.Rgba, PixelType.Float, data);
        return data;
    }
}