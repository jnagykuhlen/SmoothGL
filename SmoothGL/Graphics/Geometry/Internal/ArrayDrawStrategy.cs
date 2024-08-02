using OpenTK.Graphics.OpenGL;
using SmoothGL.Graphics.Internal;

namespace SmoothGL.Graphics.Geometry.Internal;

public class ArrayDrawStrategy : IDrawStrategy
{
    public void Draw(Primitive primitiveType, int startElement, int numberOfElements)
    {
        GL.DrawArrays((PrimitiveType)primitiveType, startElement, numberOfElements);
    }

    public void DrawMultiple(Primitive primitiveType, int startElement, int numberOfElements, int numberOfInstances)
    {
        GL.DrawArraysInstanced((PrimitiveType)primitiveType, startElement, numberOfElements, numberOfInstances);
    }
}