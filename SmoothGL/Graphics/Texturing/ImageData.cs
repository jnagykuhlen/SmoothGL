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

    public ImageData GetSection(Rectangle rectangle)
    {
        if (rectangle.X < 0 || rectangle.Y < 0 || rectangle.Width < 0 || rectangle.Height < 0 ||
            rectangle.X + rectangle.Width > Width || rectangle.Y + rectangle.Height > Height)
            throw new ArgumentException("The provided rectangle is out of bounds.", nameof(rectangle));

        var sectionData = new byte[4 * rectangle.Width * rectangle.Height];
        for (int y = 0; y < rectangle.Height; ++y)
        {
            Array.Copy(Data, 4 * ((rectangle.Y + y) * Width + rectangle.X), sectionData, 4 * y * rectangle.Width, 4 * rectangle.Width);
        }

        return new ImageData(rectangle.Width, rectangle.Height, sectionData);
    }

    public static ImageData FromStream(Stream stream)
    {
        var imageResult = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        return new ImageData(imageResult.Width, imageResult.Height, imageResult.Data);
    }

    public void WritePng(Stream stream)
    {
        new ImageWriter().WritePng(Data, Width, Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
    }

    public void WriteJpg(Stream stream, int quality)
    {
        new ImageWriter().WriteJpg(Data, Width, Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream, quality);
    }
}