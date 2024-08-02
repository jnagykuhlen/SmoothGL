using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Describes the type of integers used to represent indices in an element buffer.
/// </summary>
public enum ElementType
{
    UnsignedByte = DrawElementsType.UnsignedByte,
    UnsignedShort = DrawElementsType.UnsignedShort,
    UnsignedInt = DrawElementsType.UnsignedInt
}