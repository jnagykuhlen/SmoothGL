using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Defines a two-dimensional texture persistent in graphics memory, storing a grid of color values.
    /// </summary>
    public class ColorTexture2D : Texture2D
    {
        private class Attachment : IColorAttachment
        {
            private int _id;

            public Attachment(int id)
            {
                _id = id;
            }

            public void Attach(int index)
            {
                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.ColorAttachment0 + index,
                    TextureTarget.Texture2D,
                    _id,
                    0
                );
            }
        }

        private TextureColorFormat _format;

        /// <summary>
        /// Creates a new color texture with RGBA format and default filter mode.
        /// </summary>
        /// <param name="width">Texture width.</param>
        /// <param name="height">Texture height.</param>
        public ColorTexture2D(int width, int height)
            : this(width, height, TextureColorFormat.Rgba32, TextureFilterMode.Default) { }

        /// <summary>
        /// Creates a new color texture with specified color format and filtering.
        /// </summary>
        /// <param name="width">Texture width.</param>
        /// <param name="height">Texture height.</param>
        /// <param name="format">Specifies the format of each color value in memory.</param>
        /// <param name="filterMode">Specifies the filtering of the texture.</param>
        public ColorTexture2D(int width, int height, TextureColorFormat format, TextureFilterMode filterMode)
            : base(width, height, (PixelInternalFormat)format, PixelFormat.Rgba, PixelType.Float, filterMode)
        {
            _format = format;
        }

        /// <summary>
        /// Stores data from a bitmap in this texture. The provided bitmap must have the same size as this texture.
        /// </summary>
        /// <param name="bitmap">Bitmap to store in the texture.</param>
        public void SetData(Bitmap bitmap)
        {
            if (bitmap.Width != Width || bitmap.Height != Height)
                throw new ArgumentException("The size of the provided bitmap does not match the size of this texture.");

            Rectangle bitmapRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(bitmapRectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                Bind();
                GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)_format, Width, Height, 0, PixelFormat.Bgra, PixelType.UnsignedInt8888Reversed, bitmapData.Scan0);
                UpdateMipmaps();
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            
        }

        /// <summary>
        /// Stores color data in this texture. The provided data array must have exactly width * height elements.
        /// </summary>
        /// <param name="data">Color data to store in the texture.</param>
        public void SetData(Color4[] data)
        {
            if (data.Length != Width * Height)
                throw new ArgumentException("The provided texture data does not contain the required number of color values.");

            Bind();
            GL.TexImage2D<Color4>(TextureTarget.Texture2D, 0, (PixelInternalFormat)_format, Width, Height, 0, PixelFormat.Rgba, PixelType.Float, data);
            UpdateMipmaps();
        }
        
        /// <summary>
        /// Reads the color data stored in this texture back into client memory.
        /// </summary>
        /// <returns>Color data.</returns>
        public Color4[] GetData()
        {
            Bind();
            Color4[] data = new Color4[Width * Height];
            GL.GetTexImage<Color4>(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.Float, data);
            return data;
        }

        /// <summary>
        /// Creates a new bitmap from the color data stored in this texture.
        /// </summary>
        /// <returns>New bitmap with color data from this texture.</returns>
        public Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Rectangle bitmapRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(bitmapRectangle, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                Bind();
                GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Bgra, PixelType.UnsignedInt8888Reversed, bitmapData.Scan0);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        /// <summary>
        /// Creates a new frame buffer attachment that can be used to attach this texture to a frame buffer
        /// by calling its <see cref="FrameBuffer.Attach(IDepthStencilAttachment, IColorAttachment[])"/> method.
        /// </summary>
        /// <returns>Frame buffer attachment.</returns>
        public IColorAttachment CreateFrameBufferAttachment()
        {
            return new Attachment(Id);
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
        
        protected override string ResourceName
        {
            get
            {
                return "ColorTexture";
            }
        }
    }
}
