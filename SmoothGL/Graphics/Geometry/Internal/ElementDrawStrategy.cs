using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics.Internal
{
    public class ElementDrawStrategy : IDrawStrategy
    {
        private ElementType _elementType;
        private int _elementSize;

        public ElementDrawStrategy(ElementType elementType, int elementSize)
        {
            _elementType = elementType;
            _elementSize = elementSize;
        }

        public void Draw(Primitive primitiveType, int startElement, int numberOfElements)
        {
            GL.DrawElements((PrimitiveType)primitiveType, numberOfElements, (DrawElementsType)_elementType, new IntPtr(startElement * _elementSize));
        }

        public void DrawMultiple(Primitive primitiveType, int startElement, int numberOfElements, int numberOfInstances)
        {
            GL.DrawElementsInstanced((PrimitiveType)primitiveType, numberOfElements, (DrawElementsType)_elementType, new IntPtr(startElement * _elementSize), numberOfInstances);
        }
    }
}
