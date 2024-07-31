using System.Drawing;
using SmoothGL.Graphics;

namespace SmoothGL.Content;

/// <summary>
/// Reader class which reads a cube texture from a stream. The six cube faces are expected to be quadratic
/// and consecutively stored in the image file.
/// </summary>
public class TextureCubeReader : IContentReader<TextureCube>
{
    private readonly TextureFilterMode _filterMode;
    private readonly bool _flipVertically;
    private readonly CubeTextureLayout _layout;

    /// <summary>
    /// Creates a new texture cube reader, specifying a texture filtering mode which is applied to loaded cube textures.
    /// The layout of source image files is assumed to be a horizontal strip.
    /// </summary>
    /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
    /// <param name="flipVertically">Indicates whether the y-axis should be inverted during the read process.</param>
    public TextureCubeReader(TextureFilterMode filterMode, bool flipVertically)
        : this(filterMode, flipVertically, CubeTextureLayout.HorizontalStrip)
    {
    }

    /// <summary>
    /// Creates a new texture cube reader, specifying a texture filtering mode which is applied to loaded cube textures.
    /// The layout of source image files is assumed to be a horizontal strip with specified face order.
    /// </summary>
    /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
    /// <param name="flipVertically">Indicates whether the y-axis should be inverted during the read process.</param>
    /// <param name="cubeFaceOrder">Defines the order in which the cube faces are stored in the source image file.</param>
    public TextureCubeReader(TextureFilterMode filterMode, bool flipVertically, TextureCubeFace[] cubeFaceOrder)
        : this(filterMode, flipVertically, CubeTextureLayout.FromCubeFaceOrder(cubeFaceOrder))
    {
    }

    /// <summary>
    /// Creates a new texture cube reader, specifying a texture filtering mode which is applied to loaded cube textures
    /// as well as a layout that defines how the individual faces are arranged in the source image file.
    /// </summary>
    /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
    /// <param name="flipVertically">Indicates whether the y-axis should be inverted during the read process.</param>
    /// <param name="layout">Layout specifying how the individual faces are arranged in the source image file.</param>
    public TextureCubeReader(TextureFilterMode filterMode, bool flipVertically, CubeTextureLayout layout)
    {
        _filterMode = filterMode;
        _layout = layout;
        _flipVertically = flipVertically;
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
        using (var bitmap = new Bitmap(stream))
        {
            if (bitmap.Width % _layout.GridWidth != 0 ||
                bitmap.Height % _layout.GridHeight != 0 ||
                bitmap.Width / _layout.GridWidth != bitmap.Height / _layout.GridHeight)
            {
                string filename = null;
                var fileStream = stream as FileStream;
                if (fileStream != null)
                    filename = fileStream.Name;

                throw new ContentLoadException(
                    "The dimension of the cube texture source image is not compatible with the reader's cube texture layout.",
                    filename,
                    typeof(TextureCube)
                );
            }

            if (_flipVertically)
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            var faceWidth = bitmap.Width / _layout.GridWidth;
            var faceHeight = bitmap.Height / _layout.GridHeight;

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

                using (var faceBitmap = bitmap.Clone(faceRectangle, bitmap.PixelFormat))
                {
                    texture.SetData(faceBitmap, cubeFace);
                }
            }

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
    public string ReaderName => "TextureCubeReader";
}