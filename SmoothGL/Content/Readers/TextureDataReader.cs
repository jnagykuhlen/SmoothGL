using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Mathematics;
using SmoothGL.Graphics;


namespace SmoothGL.Content
{
    /// <summary>
    /// Reader class which reads texture data from a stream.
    /// </summary>
    public class TextureDataReader : IContentReader<TextureData>
    {
        private bool _flipVertically;
        
        /// <summary>
        /// Creates a new texture data reader.
        /// </summary>
        /// <param name="flipVertically">Indicates whether the y-axis should be inverted during the read process.</param>
        public TextureDataReader(bool flipVertically)
        {
            _flipVertically = flipVertically;
        }

        /// <summary>
        /// Reads content data from a stream.
        /// </summary>
        /// <param name="stream">Stream to read data from.</param>
        /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
        /// <param name="contentManager">Content manager used to load additional data.</param>
        /// <returns>The read object.</returns>
        public TextureData Read(Stream stream, Type requestedType, ContentManager contentManager)
        {
            using (Bitmap bitmap = new Bitmap(stream))
            {
                if(_flipVertically)
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                int width = bitmap.Width;
                int height = bitmap.Height;

                Rectangle bitmapRectangle = new Rectangle(0, 0, width, height);
                BitmapData bitmapData = bitmap.LockBits(bitmapRectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                
                try
                {
                    return CreateFromRawData(width, height, bitmapData.Scan0);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }
        }

        private TextureData CreateFromRawData(int width, int height, IntPtr data)
        {
            int numberOfPixels = width * height;
            int numberOfBytes = 4 * numberOfPixels;
            Color4[] colorData = new Color4[numberOfPixels];

            unsafe
            {
                fixed (Color4* colorStart = colorData)
                {
                    byte* scanStart = (byte*)data;
                    
                    for (int i = 0; i < numberOfPixels; ++i)
                    {
                        (*(colorStart + i)) = new Color4(
                            *(scanStart + 4 * i + 2),
                            *(scanStart + 4 * i + 1),
                            *(scanStart + 4 * i + 0),
                            *(scanStart + 4 * i + 3)
                        );
                    }
                }
            }
            
            return new TextureData(width, height, colorData);
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
                return "TextureDataReader";
            }
        }
    }
}
