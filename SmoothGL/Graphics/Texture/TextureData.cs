using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Mathematics;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents texture data in client memory, storing a number of color values of a two-dimensional texture.
    /// </summary>
    public class TextureData
    {
        private int _width;
        private int _height;
        private Color4[] _data;

        /// <summary>
        /// Creates new texture data in client memory.
        /// </summary>
        /// <param name="width">Width of the represented texture, in pixels.</param>
        /// <param name="height">Height of the represented texture, in pixels.</param>
        /// <param name="data">Color values defining this texture data.</param>
        public TextureData(int width, int height, Color4[] data)
        {
            if(data.Length != width * height)
                throw new ArgumentException("The provided texture data does not contain the required number of color values.");

            _width = width;
            _height = height;
            _data = data;
        }

        /// <summary>
        /// Gets the stored color values for the represented texture.
        /// </summary>
        /// <returns>Array of color values.</returns>
        public Color4[] GetData()
        {
            return _data;
        }

        /// <summary>
        /// Gets the stored color values for a rectangular area of the represented texture.
        /// </summary>
        /// <param name="rectX">The x-value of the starting point of the rectangular area.</param>
        /// <param name="rectY">The y-value of the starting point of the rectangular area.</param>
        /// <param name="rectWidth">The width of the rectangular area.</param>
        /// <param name="rectHeight">The height of the rectangular area.</param>
        /// <returns>Array of color values.</returns>
        public Color4[] GetData(int rectX, int rectY, int rectWidth, int rectHeight)
        {
            if (rectX < 0 || rectX + rectWidth > _width || rectY < 0 || rectY + rectHeight > _height)
                throw new ArgumentException("Cannot read subdata outside of the texture.");

            Color4[] subData = new Color4[rectWidth * rectHeight];
            for (int y = 0; y < rectHeight; ++y)
            {
                int sourceX = rectX;
                int sourceY = rectY + y;
                int sourceIndex = sourceY * _width + sourceX;
                Array.Copy(_data, sourceIndex, subData, y * rectWidth, rectWidth);
            }

            return subData;
        }

        /// <summary>
        /// Gets the width of the represented texture, in pixels.
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
        }

        /// <summary>
        /// Gets the height of the represented texture, in pixels.
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
