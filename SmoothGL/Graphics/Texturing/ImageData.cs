using System.Drawing;
using StbImageSharp;
using StbImageWriteSharp;
using ColorComponents = StbImageSharp.ColorComponents;

namespace SmoothGL.Graphics.Texturing;

/// <summary>
/// Represents binary image data in client memory, storing a number of color values of a two-dimensional image.
/// </summary>
public class ImageData
{
    static ImageData()
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        StbImageWrite.stbi_flip_vertically_on_write(1);
    }
    
    /// <summary>
    /// Creates new image data in client memory.
    /// </summary>
    /// <param name="width">Width of the represented image, in pixels.</param>
    /// <param name="height">Height of the represented image, in pixels.</param>
    /// <param name="data">Bytes defining this image data.</param>
    public ImageData(int width, int height, byte[] data)
    {
        if (data.Length != 4 * width * height)
            throw new ArgumentException("The provided data does not match width and height.", nameof(data));

        Width = width;
        Height = height;
        Data = data;
    }

    /// <summary>
    /// Gets the width of the represented image, in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the represented image, in pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the bytes defining this image data.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Gets a rectangular section of this image data as a new image data object.
    /// </summary>
    /// <param name="rectangle">Rectangle defining the section to copy.</param>
    /// <returns>Section of this image data.</returns>
    /// <exception cref="ArgumentException">Section is not within bounds of this image data.</exception>
    public ImageData GetSection(Rectangle rectangle)
    {
        if (rectangle.X < 0 || rectangle.Y < 0 || rectangle.Width < 0 || rectangle.Height < 0 ||
            rectangle.X + rectangle.Width > Width || rectangle.Y + rectangle.Height > Height)
            throw new ArgumentException("The provided rectangle is out of bounds.", nameof(rectangle));

        var sectionData = new byte[4 * rectangle.Width * rectangle.Height];
        for (var y = 0; y < rectangle.Height; ++y)
            Array.Copy(Data, 4 * ((rectangle.Y + y) * Width + rectangle.X), sectionData, 4 * y * rectangle.Width, 4 * rectangle.Width);

        return new ImageData(rectangle.Width, rectangle.Height, sectionData);
    }

    /// <summary>
    /// Load image data from a PNG or JPG stream.
    /// </summary>
    /// <param name="stream">Stream containing the image data.</param>
    /// <returns>Image data loaded from the stream.</returns>
    public static ImageData FromStream(Stream stream)
    {
        var imageResult = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        return new ImageData(imageResult.Width, imageResult.Height, imageResult.Data);
    }

    /// <summary>
    /// Writes this image data to a PNG stream.
    /// </summary>
    /// <param name="stream">Stream to which the image data is written.</param>
    public void WritePng(Stream stream) =>
        new ImageWriter().WritePng(Data, Width, Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);

    /// <summary>
    /// Writes this image data to a JPG stream.
    /// </summary>
    /// <param name="stream">Stream to which the image data is written.</param>
    /// <param name="quality">Quality of the JPG image, in the range of 1 to 100.</param>
    public void WriteJpg(Stream stream, int quality) =>
        new ImageWriter().WriteJpg(Data, Width, Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream, quality);
}