using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics.Internal
{
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
}
