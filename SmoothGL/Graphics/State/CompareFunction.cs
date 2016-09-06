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
    /// Defines a compare function for depth and stencil operations.
    /// </summary>
    public enum CompareFunction
    {
        Always = DepthFunction.Always,
        Equal = DepthFunction.Equal,
        Greater = DepthFunction.Greater,
        GreaterEqual = DepthFunction.Gequal,
        Less = DepthFunction.Less,
        LessEqual = DepthFunction.Lequal,
        Never = DepthFunction.Never,
        NotEqual = DepthFunction.Notequal
    }
}
