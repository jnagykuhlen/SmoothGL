using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics
{
    /// <summary>
    /// Determines how a texture is sampled when stretched, scaled-down or scaled-up.
    /// </summary>
    public class TextureFilterMode
    {
        /// <summary>
        /// Default texture filter mode with linear interpolation, mipmapping and no anisotropic filtering.
        /// </summary>
        public static readonly TextureFilterMode Default = new TextureFilterMode(TextureInterpolation.Linear, 1.0f, true, TextureWrap.Repeat);

        /// <summary>
        /// Filter mode that does not apply any filtering when sampling a texture.
        /// </summary>
        public static readonly TextureFilterMode None = new TextureFilterMode(TextureInterpolation.Nearest, 1.0f, false, TextureWrap.Repeat);

        private TextureInterpolation _interpolation;
        private float _anisotropy;
        private bool _mipmapping;
        private TextureWrap _wrap;

        /// <summary>
        /// Creates a new texture filter mode.
        /// </summary>
        /// <param name="interpolation">The interpolation that is applied when sampling a texture between its grid points.</param>
        /// <param name="anisotropy">Maximum number of samples used for anisotropic filtering.</param>
        /// <param name="mipmapping">Indicates whether mipmapping is enabled.</param>
        public TextureFilterMode(TextureInterpolation interpolation, float anisotropy, bool mipmapping)
            : this(interpolation, anisotropy, mipmapping, TextureWrap.Repeat) { }

        /// <summary>
        /// Creates a new texture filter mode.
        /// </summary>
        /// <param name="interpolation">The interpolation that is applied when sampling a texture between its grid points.</param>
        /// <param name="anisotropy">Maximum number of samples used for anisotropic filtering.</param>
        /// <param name="mipmapping">Indicates whether mipmapping is enabled.</param>
        /// <param name="wrap">Determines how a texture is sampled for texture coordinate outside the actual texture area.</param>
        public TextureFilterMode(TextureInterpolation interpolation, float anisotropy, bool mipmapping, TextureWrap wrap)
        {
            _interpolation = interpolation;
            _anisotropy = anisotropy;
            _mipmapping = mipmapping;
            _wrap = wrap;
        }

        /// <summary>
        /// Gets the interpolation that is applied when sampling a texture between its grid points.
        /// </summary>
        public TextureInterpolation Interpolation
        {
            get
            {
                return _interpolation;
            }
        }

        /// <summary>
        /// Gets the maximum number of samples used for anisotropic filtering.
        /// </summary>
        public float Anisotropy
        {
            get
            {
                return _anisotropy;
            }
        }

        /// <summary>
        /// Gets a value indicating whether mipmapping is enabled.
        /// </summary>
        public bool Mipmapping
        {
            get
            {
                return _mipmapping;
            }
        }

        /// <summary>
        /// Gets a value that describes how a texture is sampled for texture coordinate outside the actual texture area.
        /// </summary>
        public TextureWrap Wrap
        {
            get
            {
                return _wrap;
            }
        }
    }
}
