using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.State;

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