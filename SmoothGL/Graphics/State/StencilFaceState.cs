namespace SmoothGL.Graphics.State;

/// <summary>
/// Describes the operations which are performed dependent on the result of the stencil test,
/// for a certain face orientation.
/// </summary>
public class StencilFaceState
{
    /// <summary>
    /// Stencil face state that does not change any stencil values.
    /// </summary>
    public static readonly StencilFaceState None = new (
        StencilOperation.Keep,
        StencilOperation.Keep,
        StencilOperation.Keep,
        CompareFunction.Never
    );
    
    /// <summary>
    /// Creates a new set of stencil operations which are performed for a certain face orientation.
    /// </summary>
    /// <param name="stencilFail">Operation which is performed when the stencil test fails.</param>
    /// <param name="stencilPassDepthFail">Operation which is performed when the stencil test passes, but the depth test fails.</param>
    /// <param name="stencilPassDepthPass">Operation which is performed when both the stencil test and the depth test pass.</param>
    /// <param name="stencilFunction">
    /// Stencil value compare function which needs to evaluate to true for fragments to pass the
    /// stencil test.
    /// </param>
    public StencilFaceState(StencilOperation stencilFail,
        StencilOperation stencilPassDepthFail,
        StencilOperation stencilPassDepthPass,
        CompareFunction stencilFunction)
    {
        StencilFail = stencilFail;
        StencilPassDepthFail = stencilPassDepthFail;
        StencilPassDepthPass = stencilPassDepthPass;
        StencilFunction = stencilFunction;
    }

    /// <summary>
    /// Gets the operation which is performed when the stencil test fails.
    /// </summary>
    public StencilOperation StencilFail { get; private set; }

    /// <summary>
    /// Gets the operation which is performed when the stencil test passes, but the depth test fails.
    /// </summary>
    public StencilOperation StencilPassDepthFail { get; private set; }

    /// <summary>
    /// Gets the operation which is performed when both the stencil test and the depth test pass.
    /// </summary>
    public StencilOperation StencilPassDepthPass { get; private set; }

    /// <summary>
    /// Gets the stencil value compare function which needs to evaluate to true for fragments to pass the stencil test.
    /// </summary>
    public CompareFunction StencilFunction { get; private set; }
}