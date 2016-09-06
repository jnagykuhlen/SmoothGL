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
    /// Defines a texture persistent in graphics memory.
    /// </summary>
    public abstract class Texture : GraphicsResource
    {
        private TextureTarget _target;
        private TextureFilterMode _filterMode;
        private int _textureId;

        protected Texture(TextureTarget target, TextureFilterMode filterMode)
        {
            _target = target;
            _filterMode = filterMode;
            GL.GenTextures(1, out _textureId);
            GL.BindTexture(_target, _textureId);
            ApplyFiltering();
        }

        /// <summary>
        /// Binds this texture to the graphics device. This method is not required to be called by client code.
        /// </summary>
        public void Bind()
        {
            GL.BindTexture(_target, _textureId);
        }

        protected void UpdateMipmaps()
        {
            if (_filterMode.Mipmapping)
                GL.GenerateMipmap((GenerateMipmapTarget)_target);
        }

        private void ApplyFiltering()
        {
            Bind();

            TextureMinFilter minFilter;
            TextureMagFilter magFilter;

            if (_filterMode.Interpolation == TextureInterpolation.Nearest)
            {
                minFilter = _filterMode.Mipmapping ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest;
                magFilter = TextureMagFilter.Nearest;
            }
            else
            {
                minFilter = _filterMode.Mipmapping ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear;
                magFilter = TextureMagFilter.Linear;
            }

            GL.TexParameter(_target, TextureParameterName.TextureMinFilter, (float)minFilter);
            GL.TexParameter(_target, TextureParameterName.TextureMagFilter, (float)magFilter);

            float maxAnisotropy;
            GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAnisotropy);

            float anisotropy = MathHelper.Clamp(_filterMode.Anisotropy, 1.0f, maxAnisotropy);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, anisotropy);
        }

        protected sealed override void FreeResources()
        {
            GL.DeleteTextures(1, ref _textureId);
        }
        
        /// <summary>
        /// Gets the filter mode of this texture.
        /// </summary>
        public TextureFilterMode FilterMode
        {
            get
            {
                return _filterMode;
            }
        }

        protected int Id
        {
            get
            {
                return _textureId;
            }
        }

        protected override string ResourceName
        {
            get
            {
                return "Texture";
            }
        }
    }
}
