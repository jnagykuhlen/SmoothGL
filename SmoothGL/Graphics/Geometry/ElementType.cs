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
    /// Describes the type of integers used to represent indices in an element buffer.
    /// </summary>
    public enum ElementType
    {
        UnsignedByte = DrawElementsType.UnsignedByte,
        UnsignedShort = DrawElementsType.UnsignedShort,
        UnsignedInt = DrawElementsType.UnsignedInt
    }
}
