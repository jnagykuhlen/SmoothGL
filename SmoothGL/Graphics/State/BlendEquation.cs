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
    /// Defines possible blending equations.
    /// </summary>
    public enum BlendEquation
    {
        Add = BlendEquationMode.FuncAdd,
        Subtract = BlendEquationMode.FuncSubtract,
        ReverseSubtract = BlendEquationMode.FuncReverseSubtract,
        Min = BlendEquationMode.Min,
        Max = BlendEquationMode.Max
    }
}
