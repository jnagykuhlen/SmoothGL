using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SmoothGL.Graphics;


namespace SmoothGL.Content
{
    /// <summary>
    /// Reader class which reads a cube texture from a stream. The six cube faces are expected to be quadratic
    /// and consecutively stored in the image file.
    /// </summary>
    public class TextureCubeReader : IContentReader<TextureCube>
    {
        private TextureFilterMode _filterMode;
        private TextureCubeFace[] _cubeFaceOrder;
        private bool _flipVertically;

        /// <summary>
        /// Creates a new texture cube reader, specifying a texture filtering mode which is applied to loaded cube textures.
        /// </summary>
        /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
        /// <param name="flipVertically">Indicates whether the y-axis should be inverted during the read process.</param>
        public TextureCubeReader(TextureFilterMode filterMode, bool flipVertically)
            : this(filterMode, flipVertically, DefaultCubeFaceOrder) { }

        /// <summary>
        /// Creates a new texture cube reader, specifying a texture filtering mode which is applied to loaded cube textures.
        /// </summary>
        /// <param name="filterMode">Defines the filtering mode applied to all loaded textures.</param>
        /// <param name="flipVertically">Indicates whether the y-axis should be inverted during the read process.</param>
        /// <param name="cubeFaceOrder">Defines the order in which the cube faces are stored in the image file.</param>
        public TextureCubeReader(TextureFilterMode filterMode, bool flipVertically, TextureCubeFace[] cubeFaceOrder)
        {
            if (cubeFaceOrder.Length != 6)
                throw new ArgumentException("CubeFaceOrder array must contain exactly six values.", "cubeFaceOrder");

            _filterMode = filterMode;
            _cubeFaceOrder = cubeFaceOrder;
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
            using (Bitmap bitmap = new Bitmap(stream))
            {
                if (bitmap.Width != bitmap.Height * 6)
                {
                    string filename = "";
                    FileStream fileStream = stream as FileStream;
                    if (fileStream != null)
                        filename = fileStream.Name;

                    throw new ContentLoadException("The cube texture source image's width must exactly be six times its height.", filename, typeof(TextureCube));
                }

                if(_flipVertically)
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                int faceWidth = bitmap.Width / 6;
                int faceHeight = bitmap.Height;
                
                TextureCube texture = new TextureCube(faceWidth, faceHeight, TextureColorFormat.Rgba32, _filterMode);

                for (int i = 0; i < 6; ++i)
                {
                    Rectangle faceRectangle = new Rectangle(i * faceWidth, 0, faceWidth, faceHeight);
                    using (Bitmap faceBitmap = bitmap.Clone(faceRectangle, bitmap.PixelFormat))
                    {
                        texture.SetData(faceBitmap, _cubeFaceOrder[i]);
                    }
                }

                return texture;
            }
        }

        private static TextureCubeFace[] DefaultCubeFaceOrder
        {
            get
            {
                TextureCubeFace[] result = new TextureCubeFace[6];
                for (int i = 0; i < 6; ++i)
                    result[i] = (TextureCubeFace)i;
                return result;
            }
        }

        /// <summary>
        /// Indicates whether this class can also read subtypes of the specified type.
        /// </summary>
        public bool CanReadSubtypes
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of this reader.
        /// </summary>
        public string ReaderName
        {
            get
            {
                return "ColorTextureReader";
            }
        }
    }
}
