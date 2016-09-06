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
    /// Represents a two-dimensional texture persistent in graphics memory, storing a 24-bit depth value and an 8-bit stencil
    /// value per pixel. Single depth-stencil textures can be bound to frame buffers to allow depth and stencil testing.
    /// Unlike the <see cref="DepthStencilBuffer"/>, this texture can be assigned to a shader uniform when not attached
    /// to an active frame buffer, allowing direct access to stored depth and stencil values.
    /// </summary>
    public class DepthStencilTexture2D : Texture2D
    {
        private class Attachment : IDepthStencilAttachment
        {
            private int _id;

            public Attachment(int id)
            {
                _id = id;
            }

            public void Attach()
            {
                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.DepthStencilAttachment,
                    TextureTarget.Texture2D,
                    _id,
                    0
                );
            }
        }

        /// <summary>
        /// Creates a new depth-stencil texture.
        /// </summary>
        /// <param name="width">Texture width.</param>
        /// <param name="height">Texture height.</param>
        public DepthStencilTexture2D(int width, int height)
            : base(width, height, PixelInternalFormat.Depth24Stencil8, PixelFormat.DepthStencil, PixelType.UnsignedInt248, TextureFilterMode.None) { }

        /// <summary>
        /// Creates a new frame buffer attachment that can be used to attach this texture to a frame buffer
        /// by calling its <see cref="FrameBuffer.Attach(IDepthStencilAttachment, IColorAttachment[])"/> method. This allows for
        /// depth and stencil testing when the frame buffer is set as target.
        /// </summary>
        /// <returns>Frame buffer attachment.</returns>
        public IDepthStencilAttachment CreateFrameBufferAttachment()
        {
            return new Attachment(Id);
        }
        
        protected override string ResourceName
        {
            get
            {
                return "DepthStencilTexture";
            }
        }
    }
}
