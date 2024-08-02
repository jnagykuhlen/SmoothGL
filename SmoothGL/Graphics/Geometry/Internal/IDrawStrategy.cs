namespace SmoothGL.Graphics.Geometry.Internal;

public interface IDrawStrategy
{
    void Draw(Primitive primitiveType, int startElement, int numberOfElements);
    void DrawMultiple(Primitive primitiveType, int startElement, int numberOfElements, int numberOfInstances);
}