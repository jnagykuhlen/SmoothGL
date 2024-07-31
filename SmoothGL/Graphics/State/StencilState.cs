using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
///     Encapsulates rendering state variables related to the rasterizer stage. Rendering state objects are immutable.
/// </summary>
public class StencilState : IGraphicsState
{
    /// <summary>
    ///     Stencil state which does not perform any stencil testing.
    /// </summary>
    public static readonly StencilState None = new(false, null, null, 0, 0, 0);

    /// <summary>
    ///     Creates a new stencil state object.
    /// </summary>
    /// <param name="stencilTestEnabled">indicates whether stencil testing is enabled.</param>
    /// <param name="stencilFrontState">
    ///     Operations which are performed for front faces dependent on the result of the stencil
    ///     test.
    /// </param>
    /// <param name="stencilBackState">
    ///     Operations which are performed for back faces dependent on the result of the stencil
    ///     test.
    /// </param>
    /// <param name="referenceStencil">
    ///     reference stencil value, which is written to the stencil buffer when performing
    ///     <see cref="StencilOperation.Replace" />.
    /// </param>
    /// <param name="stencilReadMask">Bit mask defining which bits are read from the stencil buffer.</param>
    /// <param name="stencilWriteMask">Bit mask defining which bits are written to the stencil buffer.</param>
    public StencilState(bool stencilTestEnabled,
        StencilFaceState stencilFrontState,
        StencilFaceState stencilBackState,
        int referenceStencil,
        int stencilReadMask,
        int stencilWriteMask)
    {
        StencilTestEnabled = stencilTestEnabled;
        StencilFrontState = stencilFrontState;
        StencilBackState = stencilBackState;
        ReferenceStencil = referenceStencil;
        StencilReadMask = stencilReadMask;
        StencilWriteMask = stencilWriteMask;
    }

    /// <summary>
    ///     Gets a value indicating whether stencil testing is enabled.
    /// </summary>
    public bool StencilTestEnabled { get; }

    /// <summary>
    ///     Gets the operations which are performed for front faces dependent on the result of the stencil test.
    /// </summary>
    public StencilFaceState StencilFrontState { get; }

    /// <summary>
    ///     Gets the operations which are performed for back faces dependent on the result of the stencil test.
    /// </summary>
    public StencilFaceState StencilBackState { get; }

    /// <summary>
    ///     Gets the reference stencil value, which is written to the stencil buffer when performing
    ///     <see cref="StencilOperation.Replace" />.
    /// </summary>
    public int ReferenceStencil { get; }

    /// <summary>
    ///     Gets a bit mask defining which bits are read from the stencil buffer.
    /// </summary>
    public int StencilReadMask { get; }

    /// <summary>
    ///     Gets a bit mask defining which bits are written to the stencil buffer.
    /// </summary>
    public int StencilWriteMask { get; }

    /// <summary>
    ///     Communicates the state encapsulated in this state object to the driver.
    /// </summary>
    public void Apply()
    {
        if (StencilTestEnabled)
        {
            GL.Enable(EnableCap.StencilTest);
            GL.StencilOpSeparate(
                StencilFace.Front,
                (StencilOp)StencilFrontState.StencilFail,
                (StencilOp)StencilFrontState.StencilPassDepthFail,
                (StencilOp)StencilFrontState.StencilPassDepthPass
            );
            GL.StencilOpSeparate(
                StencilFace.Back,
                (StencilOp)StencilBackState.StencilFail,
                (StencilOp)StencilBackState.StencilPassDepthFail,
                (StencilOp)StencilBackState.StencilPassDepthPass
            );
            GL.StencilFuncSeparate(
                StencilFace.Front,
                (StencilFunction)StencilFrontState.StencilFunction,
                ReferenceStencil,
                StencilReadMask
            );
            GL.StencilFuncSeparate(
                StencilFace.Back,
                (StencilFunction)StencilBackState.StencilFunction,
                ReferenceStencil,
                StencilReadMask
            );
            GL.StencilMask(StencilWriteMask);
        }
        else
        {
            GL.Disable(EnableCap.StencilTest);
        }
    }
}