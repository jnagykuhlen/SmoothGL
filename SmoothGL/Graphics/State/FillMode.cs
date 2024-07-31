using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Defines how faces are filled for rasterization.
/// </summary>
public enum FillMode
{
    Solid = PolygonMode.Fill,
    Wireframe = PolygonMode.Line
}