using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads image data from a stream.
/// </summary>
public class ImageDataReader : IContentReader<ImageData>
{
    /// <summary>
    /// Reads content data from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    public ImageData Read(Stream stream, Type requestedType, ContentManager contentManager) =>
        ImageData.FromStream(stream);

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;
}