using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
///     Describes how data in a vertex buffer is interpreted for floating point elements.
/// </summary>
public enum FloatSourceType
{
    Byte = VertexAttribPointerType.Byte,
    Short = VertexAttribPointerType.Short,
    Int = VertexAttribPointerType.Int,
    UnsignedByte = VertexAttribPointerType.UnsignedByte,
    UnsignedShort = VertexAttribPointerType.UnsignedShort,
    UnsignedInt = VertexAttribPointerType.UnsignedInt,
    Half = VertexAttribPointerType.HalfFloat,
    Float = VertexAttribPointerType.Float,
    Double = VertexAttribPointerType.Double
}