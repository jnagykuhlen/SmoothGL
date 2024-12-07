using System.Drawing;
using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads a cube texture from a stream. The six cube faces are expected to be quadratic
/// and consecutively stored in the image file.
/// </summary>
/// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
/// <param name="layout">Layout specifying how the individual faces are arranged in the source image file.</param>
public class TextureCubeReader(TextureFilterMode filterMode, CubeTextureLayout layout) : ContentReader<TextureCube>
{
    /// <summary>
    /// Creates a new texture cube reader, specifying a texture filtering mode which is applied to loaded cube textures.
    /// The layout of source image files is assumed to be a horizontal strip.
    /// </summary>
    /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
    public TextureCubeReader(TextureFilterMode filterMode)
        : this(filterMode, CubeTextureLayout.HorizontalStrip)
    {
    }

    /// <summary>
    /// Creates a new texture cube reader, specifying a texture filtering mode which is applied to loaded cube textures.
    /// The layout of source image files is assumed to be a horizontal strip with specified face order.
    /// </summary>
    /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
    /// <param name="cubeFaceOrder">Defines the order in which the cube faces are stored in the source image file.</param>
    public TextureCubeReader(TextureFilterMode filterMode, TextureCubeFace[] cubeFaceOrder)
        : this(filterMode, CubeTextureLayout.FromCubeFaceOrder(cubeFaceOrder))
    {
    }
    
    protected override TextureCube Read(Stream stream, IContentProvider contentProvider)
    {
        var imageData = contentProvider.Load<ImageData>(stream);

        if (imageData.Width % layout.GridWidth != 0 ||
            imageData.Height % layout.GridHeight != 0 ||
            imageData.Width / layout.GridWidth != imageData.Height / layout.GridHeight)
        {
            throw new ContentLoadException(
                "The dimension of the cube texture source image is not compatible with the reader's cube texture layout.",
                stream,
                typeof(TextureCube)
            );
        }


        var faceWidth = imageData.Width / layout.GridWidth;
        var faceHeight = imageData.Height / layout.GridHeight;

        var texture = new TextureCube(faceWidth, faceHeight, TextureColorFormat.Rgba32, filterMode);

        for (var i = 0; i < 6; ++i)
        {
            var cubeFace = (TextureCubeFace)i;
            var element = layout.GetElement(cubeFace);

            var faceRectangle = new Rectangle(
                element.GridX * faceWidth,
                element.GridY * faceHeight,
                faceWidth,
                faceHeight
            );


            var faceImageData = imageData.GetSection(faceRectangle);
            texture.SetData(faceImageData, cubeFace);
        }

        return texture;
    }
}