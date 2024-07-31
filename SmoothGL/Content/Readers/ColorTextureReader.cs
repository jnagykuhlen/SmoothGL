using System.Drawing;
using SmoothGL.Graphics;

namespace SmoothGL.Content;

/// <summary>
/// Reader class which reads a color texture from a stream.
/// </summary>
public class ColorTextureReader : IContentReader<ColorTexture2D>
{
    private readonly TextureFilterMode _filterMode;
    private readonly bool _flipVertically;

    /// <summary>
    /// Creates a new color texture reader, specifying a texture filtering mode which is applied to loaded color textures.
    /// </summary>
    /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
    /// <param name="flipVertically">Indicates whether the y-axis should be inverted during the read process.</param>
    public ColorTextureReader(TextureFilterMode filterMode, bool flipVertically)
    {
        _filterMode = filterMode;
        _flipVertically = flipVertically;
    }

    /// <summary>
    /// Reads content data from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    public ColorTexture2D Read(Stream stream, Type requestedType, ContentManager contentManager)
    {
        using (var bitmap = new Bitmap(stream))
        {
            if (_flipVertically)
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            var texture = new ColorTexture2D(bitmap.Width, bitmap.Height, TextureColorFormat.Rgba32, _filterMode);
            texture.SetData(bitmap);
            return texture;
        }
    }

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;

    /// <summary>
    /// Gets the name of this reader.
    /// </summary>
    public string ReaderName => "ColorTextureReader";
}