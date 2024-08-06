using System.Drawing;
using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads a cube texture from a stream. The six cube faces are expected to be quadratic
/// and consecutively stored in the image file.
/// </summary>
public class TextureCubeReader : IContentReader<TextureCube>
{
    private readonly TextureFilterMode _filterMode;
    private readonly CubeTextureLayout _layout;

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

    /// <summary>
    /// Creates a new texture cube reader, specifying a texture filtering mode which is applied to loaded cube textures
    /// as well as a layout that defines how the individual faces are arranged in the source image file.
    /// </summary>
    /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
    /// <param name="layout">Layout specifying how the individual faces are arranged in the source image file.</param>
    public TextureCubeReader(TextureFilterMode filterMode, CubeTextureLayout layout)
    {
        _filterMode = filterMode;
        _layout = layout;
    }

    /// <summary>
    /// Reads a cube texture from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    public TextureCube Read(Stream stream, Type requestedType, ContentManager contentManager)
    {
        var imageData = contentManager.Load<ImageData>(stream);

        if (imageData.Width % _layout.GridWidth != 0 ||
            imageData.Height % _layout.GridHeight != 0 ||
            imageData.Width / _layout.GridWidth != imageData.Height / _layout.GridHeight)
        {
            throw new ContentLoadException(
                "The dimension of the cube texture source image is not compatible with the reader's cube texture layout.",
                stream,
                typeof(TextureCube)
            );
        }


        var faceWidth = imageData.Width / _layout.GridWidth;
        var faceHeight = imageData.Height / _layout.GridHeight;

        var texture = new TextureCube(faceWidth, faceHeight, TextureColorFormat.Rgba32, _filterMode);

        for (var i = 0; i < 6; ++i)
        {
            var cubeFace = (TextureCubeFace)i;
            var element = _layout.GetElement(cubeFace);

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

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;

    /// <summary>
    /// Gets the name of this reader.
    /// </summary>
    public string ReaderName => "TextureCubeReader";
}