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
    /// Defines the orientation of faces which are culled during rendering.
    /// </summary>
    public enum CullMode
    {
        None,
        Back = CullFaceMode.Back,
        Front = CullFaceMode.Front
    }
}
