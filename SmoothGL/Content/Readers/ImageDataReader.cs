using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads image data from a stream.
/// </summary>
public class ImageDataReader : ContentReader<ImageData>
{
    protected override ImageData Read(Stream stream, IContentProvider contentProvider) => ImageData.FromStream(stream);
}