using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Texturing;

/// <summary>
/// Defines a two-dimensional texture persistent in graphics memory, storing a grid of data.
/// </summary>
public abstract class Texture2D : Texture
{
    protected Texture2D(int width, int height, PixelInternalFormat internalFormat, PixelFormat format, PixelType type, TextureFilterMode filterMode)
        : base(TextureTarget.Texture2D, filterMode)
    {
        Width = width;
        Height = height;
        GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, type, IntPtr.Zero);
    }

    protected override string ResourceName => "Texture2D";

    /// <summary>
    /// Gets the width of this texture in pixels.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Gets the height of this texture in pixels.
    /// </summary>
    public int Height { get; private set; }

    protected void HotSwap(Texture2D otherTexture)
    {
        base.HotSwap(otherTexture);
        Width = otherTexture.Width;
        Height = otherTexture.Height;
    }
}