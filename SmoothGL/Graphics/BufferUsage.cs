using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a hint for the driver concerning the frequency the data in a buffer is expected to change.
    /// </summary>
    public enum BufferUsage
    {
        /// <summary>
        /// Hint that the buffer content is not changed.
        /// </summary>
        Static = BufferUsageHint.StaticDraw,

        /// <summary>
        /// Hint that the buffer content might change.
        /// </summary>
        Dynamic = BufferUsageHint.DynamicDraw,

        /// <summary>
        /// Hint that the buffer content will change frequently.
        /// </summary>
        Stream = BufferUsageHint.StreamDraw
    }
}
