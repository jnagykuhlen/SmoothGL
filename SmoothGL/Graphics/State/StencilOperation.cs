using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
///     Defines possible operations which can be applied to a value in the stencil buffer.
/// </summary>
public enum StencilOperation
{
    Keep = StencilOp.Keep,
    Zero = StencilOp.Zero,
    Increment = StencilOp.IncrWrap,
    Decrement = StencilOp.DecrWrap,
    IncrementClamp = StencilOp.Incr,
    DecrementClamp = StencilOp.Decr,
    Invert = StencilOp.Invert,
    Replace = StencilOp.Replace
}