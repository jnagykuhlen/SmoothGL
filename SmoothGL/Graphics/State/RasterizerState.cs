using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Encapsulates rendering state variables related to the rasterizer stage. Rendering state objects are immutable.
/// </summary>
public class RasterizerState : IGraphicsState
{
    /// <summary>
    /// Default rasterizer state which culls back faces.
    /// </summary>
    public static readonly RasterizerState Default = new(CullMode.Back, FillMode.Solid);

    /// <summary>
    /// Rasterizer state which does not cull any faces.
    /// </summary>
    public static readonly RasterizerState CullNone = new(CullMode.None, FillMode.Solid);

    /// <summary>
    /// Rasterizer state which draws front and back faces as wireframe.
    /// </summary>
    public static readonly RasterizerState Wireframe = new(CullMode.None, FillMode.Wireframe);

    /// <summary>
    /// Creates a new rasterizer state object.
    /// </summary>
    /// <param name="cullMode">Defines the orientation of faces which are culled during rendering.</param>
    /// <param name="fillMode">Defines how faces are filled for rasterization.</param>
    public RasterizerState(CullMode cullMode, FillMode fillMode)
    {
        CullMode = cullMode;
        FillMode = fillMode;
    }

    /// <summary>
    /// Gets the orientation of faces which are culled during rendering.
    /// </summary>
    public CullMode CullMode { get; }

    /// <summary>
    /// Gets the value defining how faces are filled for rasterization.
    /// </summary>
    public FillMode FillMode { get; }

    /// <summary>
    /// Communicates the state encapsulated in this state object to the driver.
    /// </summary>
    public void Apply()
    {
        if (CullMode == CullMode.None)
        {
            GL.Disable(EnableCap.CullFace);
        }
        else
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace((CullFaceMode)CullMode);
        }

        GL.PolygonMode(MaterialFace.FrontAndBack, (PolygonMode)FillMode);
    }
}