using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.State;

/// <summary>
/// Defines the orientation of faces which are culled during rendering.
/// </summary>
public enum CullMode
{
    None,
    Back = CullFaceMode.Back,
    Front = CullFaceMode.Front
}