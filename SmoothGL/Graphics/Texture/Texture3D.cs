using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Defines a three-dimensional texture persistent in graphics memory, storing a volume of color values (voxels).
    /// </summary>
    public class Texture3D : Texture
    {
        private int _width;
        private int _height;
        private int _depth;
        private TextureColorFormat _format;

        /// <summary>
        /// Creates a new three-dimensional color texture with RGBA format and default filter mode.
        /// </summary>
        /// <param name="width">Texture width.</param>
        /// <param name="height">Texture height.</param>
        /// <param name="depth">Texture depth.</param>
        public Texture3D(int width, int height, int depth)
            : this(width, height, depth, TextureColorFormat.Rgba32, TextureFilterMode.Default)
        {
        }

        /// <summary>
        /// Creates a new three-dimensional color texture with specified color format and filtering.
        /// </summary>
        /// <param name="width">Texture width.</param>
        /// <param name="height">Texture height.</param>
        /// <param name="depth">Texture depth.</param>
        /// <param name="format">Specifies the format of each color value in memory.</param>
        /// <param name="filterMode">Specifies the filtering of the texture.</param>
        public Texture3D(int width, int height, int depth, TextureColorFormat format, TextureFilterMode filterMode)
            : base(TextureTarget.Texture3D, filterMode)
        {
            _width = width;
            _height = height;
            _depth = depth;
            _format = format;
        }

        /// <summary>
        /// Stores color data in this texture. The provided data array must have exactly width * height * depth elements.
        /// </summary>
        /// <param name="data">Color data to store in the texture.</param>
        public void SetData(Color4[] data)
        {
            if (data.Length != _width * _height * _depth)
                throw new ArgumentException("The provided texture data does not contain the required number of color values.");

            Bind();
            GL.TexImage3D<Color4>(TextureTarget.Texture3D, 0, (PixelInternalFormat)Format, _width, _height, _depth, 0, PixelFormat.Rgba, PixelType.Float, data);
            UpdateMipmaps();
        }

        /// <summary>
        /// Reads the color data stored in this texture back into client memory.
        /// </summary>
        /// <returns>Color data.</returns>
        public Color4[] GetData()
        {
            Bind();
            Color4[] data = new Color4[_width * _height * _depth];
            GL.GetTexImage<Color4>(TextureTarget.Texture3D, 0, PixelFormat.Rgba, PixelType.Float, data);
            return data;
        }

        /// <summary>
        /// Gets the width of this texture.
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
        }

        /// <summary>
        /// Gets the height of this texture.
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// Gets the depth of this texture.
        /// </summary>
        public int Depth
        {
            get
            {
                return _depth;
            }
        }

        /// <summary>
        /// Gets the format of each color value in memory.
        /// </summary>
        public TextureColorFormat Format
        {
            get
            {
                return _format;
            }
        }
    }
}
