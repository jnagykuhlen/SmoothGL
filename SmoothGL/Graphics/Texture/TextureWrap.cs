using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics
{
    /// <summary>
    /// Determines how a texture is sampled for texture coordinate values outside the range between
    /// zero and one, i.e., for samples taken outside the actual texture area.
    /// </summary>
    public enum TextureWrap
    {
        /// <summary>
        /// The texture is repeated along all dimensions, resulting in a regular tiling pattern.
        /// </summary>
        Repeat = TextureWrapMode.Repeat,

        /// <summary>
        /// The texture is repeated along all dimensions, where every second duplicate is mirrored.
        /// </summary>
        MirroredRepeat = TextureWrapMode.MirroredRepeat,

        /// <summary>
        /// Samples at coordinates outside the range between zero and one are taken at the nearest
        /// point within the actual texture area, i.e., the closest coordinate on the edge
        /// of the texture.
        /// </summary>
        Clamp = TextureWrapMode.ClampToEdge
    }
}
