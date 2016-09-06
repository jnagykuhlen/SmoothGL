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
    /// Defines how faces are filled for rasterization.
    /// </summary>
    public enum FillMode
    {
        Solid = PolygonMode.Fill,
        Wireframe = PolygonMode.Line 
    }
}
