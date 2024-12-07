using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads a color texture from a stream.
/// </summary>
/// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
public class ColorTextureReader(TextureFilterMode filterMode) : ContentReader<ColorTexture2D>
{
    protected override ColorTexture2D Read(Stream stream, IContentProvider contentProvider)
    {
        var imageData = contentProvider.Load<ImageData>(stream);
        var texture = new ColorTexture2D(imageData.Width, imageData.Height, TextureColorFormat.Rgba32, filterMode);
        texture.SetImageData(imageData);
        return texture;
    }
}