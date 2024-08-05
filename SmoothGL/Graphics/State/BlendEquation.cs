using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.State;

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