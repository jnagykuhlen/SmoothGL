using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads a color texture from a stream.
/// </summary>
/// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
public class Texture2DReader(TextureFilterMode filterMode) : ContentReader<Texture2D>
{
    protected override Texture2D Read(Stream stream, IContentProvider contentProvider)
    {
        var imageData = contentProvider.Load<ImageData>(stream);
        var texture = new Texture2D(imageData.Width, imageData.Height, TextureColorFormat.Rgba32, filterMode);
        texture.SetImageData(imageData);
        return texture;
    }
}