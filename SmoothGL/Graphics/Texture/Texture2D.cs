using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Defines a two-dimensional texture persistent in graphics memory, storing a grid of data.
    /// </summary>
    public abstract class Texture2D : Texture
    {
        private int _width;
        private int _height;

        protected Texture2D(int width, int height, PixelInternalFormat internalFormat, PixelFormat format, PixelType type, TextureFilterMode filterMode)
            : base(TextureTarget.Texture2D, filterMode)
        {
            _width = width;
            _height = height;
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, type, IntPtr.Zero);
        }

        protected override string ResourceName
        {
            get
            {
                return "Texture2D";
            }
        }

        /// <summary>
        /// Gets the width of this texture in pixels.
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
        }

        /// <summary>
        /// Gets the height of this texture in pixels.
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
        }
    }
}
