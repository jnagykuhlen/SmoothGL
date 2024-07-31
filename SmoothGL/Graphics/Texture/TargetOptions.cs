using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Specifies the buffers which should be affected by a frame buffer operation.
/// </summary>
[Flags]
public enum TargetOptions
{
    /// <summary>
    /// The operation affects all attached color buffers.
    /// </summary>
    Color = ClearBufferMask.ColorBufferBit,

    /// <summary>
    /// The operation affects the depth buffer, if attached.
    /// </summary>
    Depth = ClearBufferMask.DepthBufferBit,

    /// <summary>
    /// The operation affects the stencil buffer, if attached.
    /// </summary>
    Stencil = ClearBufferMask.StencilBufferBit,

    /// <summary>
    /// The operation affects color, depth and stencil buffers.
    /// </summary>
    All = Color | Depth | Stencil
}