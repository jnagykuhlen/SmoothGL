using OpenTK.Graphics.OpenGL;
using SmoothGL.Graphics.Internal;

namespace SmoothGL.Graphics.Geometry.Internal;

public class ElementDrawStrategy(ElementType elementType, int elementSize) : IDrawStrategy
{
    public void Draw(Primitive primitiveType, int startElement, int numberOfElements)
    {
        GL.DrawElements((PrimitiveType)primitiveType, numberOfElements, (DrawElementsType)elementType, new IntPtr(startElement * elementSize));
    }

    public void DrawMultiple(Primitive primitiveType, int startElement, int numberOfElements, int numberOfInstances)
    {
        GL.DrawElementsInstanced((PrimitiveType)primitiveType, numberOfElements, (DrawElementsType)elementType, new IntPtr(startElement * elementSize), numberOfInstances);
    }
}