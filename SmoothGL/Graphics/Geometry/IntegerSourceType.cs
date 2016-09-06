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
    /// Describes how data in a vertex buffer is interpreted for integer elements.
    /// </summary>
    public enum IntegerSourceType
    {
        Byte = VertexAttribIPointerType.Byte,
        Short = VertexAttribIPointerType.Short,
        Int = VertexAttribIPointerType.Int,
        UnsignedByte = VertexAttribIPointerType.UnsignedByte,
        UnsignedShort = VertexAttribIPointerType.UnsignedShort,
        UnsignedInt = VertexAttribIPointerType.UnsignedInt,
    }
}
