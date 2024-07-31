using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
///     Encapsulates rendering state variables related to blending. Rendering state objects are immutable.
/// </summary>
public class BlendState : IGraphicsState
{
    /// <summary>
    ///     Defines blending which overwrites all four destination channels with source channels.
    /// </summary>
    public static readonly BlendState Opaque = new(
        BlendEquation.Add,
        BlendEquation.Add,
        Blend.One,
        Blend.Zero,
        Blend.One,
        Blend.Zero,
        ColorWriteChannels.All
    );

    /// <summary>
    ///     Defines alpha blending usually used for transparency. The source color is weighted with the source alpha.
    /// </summary>
    public static readonly BlendState AlphaNonPremultiplied = new(
        BlendEquation.Add,
        BlendEquation.Add,
        Blend.SourceAlpha,
        Blend.SourceAlphaInverse,
        Blend.SourceAlpha,
        Blend.SourceAlphaInverse,
        ColorWriteChannels.All
    );

    /// <summary>
    ///     Defines alpha blending usually used for transparency.
    ///     The source color is expected to be premultiplied with the source alpha in the fragment shader.
    /// </summary>
    public static readonly BlendState AlphaPremultiplied = new(
        BlendEquation.Add,
        BlendEquation.Add,
        Blend.One,
        Blend.SourceAlphaInverse,
        Blend.One,
        Blend.SourceAlphaInverse,
        ColorWriteChannels.All
    );

    /// <summary>
    ///     Defines blending which adds all four source and destination channels per component.
    /// </summary>
    public static readonly BlendState Additive = new(
        BlendEquation.Add,
        BlendEquation.Add,
        Blend.One,
        Blend.One,
        Blend.One,
        Blend.One,
        ColorWriteChannels.All
    );

    /// <summary>
    ///     Defines blending which multiplies all four source and destination channels per component.
    /// </summary>
    public static readonly BlendState Multiplicative = new(
        BlendEquation.Add,
        BlendEquation.Add,
        Blend.DestinationColor,
        Blend.Zero,
        Blend.DestinationAlpha,
        Blend.Zero,
        ColorWriteChannels.All
    );

    /// <summary>
    ///     Defines blending which ignores the source color entirely, without changing the current frame buffer.
    /// </summary>
    public static readonly BlendState None = new(
        BlendEquation.Add,
        BlendEquation.Add,
        Blend.One,
        Blend.One,
        Blend.Zero,
        Blend.Zero,
        ColorWriteChannels.None
    );

    /// <summary>
    ///     Creates a new blending state object.
    /// </summary>
    /// <param name="colorBlendEquation">The blending equation used for color blending.</param>
    /// <param name="alphaBlendEquation">The blending equation used for alpha blending.</param>
    /// <param name="colorSourceBlend">The factor of the source color in the blending equation.</param>
    /// <param name="colorDestinationBlend">The factor of the destination color in the blending equation.</param>
    /// <param name="alphaSourceBlend">The factor of the source alpha in the blending equation.</param>
    /// <param name="alphaDestinationBlend">The factor of the destination alpha in the blending equation.</param>
    /// <param name="colorWriteChannels">
    ///     The color channels in the current frame buffer which are affected by blending
    ///     operations.
    /// </param>
    public BlendState(BlendEquation colorBlendEquation,
        BlendEquation alphaBlendEquation,
        Blend colorSourceBlend,
        Blend colorDestinationBlend,
        Blend alphaSourceBlend,
        Blend alphaDestinationBlend,
        ColorWriteChannels colorWriteChannels)
    {
        ColorBlendEquation = colorBlendEquation;
        AlphaBlendEquation = alphaBlendEquation;
        ColorSourceBlend = colorSourceBlend;
        ColorDestinationBlend = colorDestinationBlend;
        AlphaSourceBlend = alphaSourceBlend;
        AlphaDestinationBlend = alphaDestinationBlend;
        ColorWriteChannels = colorWriteChannels;
    }

    /// <summary>
    ///     Gets the blending equation used for color blending.
    /// </summary>
    public BlendEquation ColorBlendEquation { get; }

    /// <summary>
    ///     Gets the blending equation used for alpha blending.
    /// </summary>
    public BlendEquation AlphaBlendEquation { get; }

    /// <summary>
    ///     Gets the factor of the source color in the blending equation.
    /// </summary>
    public Blend ColorSourceBlend { get; }

    /// <summary>
    ///     Gets the factor of the destination color in the blending equation.
    /// </summary>
    public Blend ColorDestinationBlend { get; }

    /// <summary>
    ///     Gets the factor of the source alpha in the blending equation.
    /// </summary>
    public Blend AlphaSourceBlend { get; }

    /// <summary>
    ///     Gets the factor of the destination alpha in the blending equation.
    /// </summary>
    public Blend AlphaDestinationBlend { get; }

    /// <summary>
    ///     Gets the color channels in the current frame buffer which are affected by blending operations.
    /// </summary>
    public ColorWriteChannels ColorWriteChannels { get; }

    /// <summary>
    ///     Communicates the state encapsulated in this state object to the driver.
    /// </summary>
    public void Apply()
    {
        GL.Enable(EnableCap.Blend);

        if (ColorWriteChannels != ColorWriteChannels.None)
        {
            GL.BlendEquationSeparate(
                (BlendEquationMode)ColorBlendEquation,
                (BlendEquationMode)AlphaBlendEquation
            );
            GL.BlendFuncSeparate(
                (BlendingFactorSrc)ColorSourceBlend,
                (BlendingFactorDest)ColorDestinationBlend,
                (BlendingFactorSrc)AlphaSourceBlend,
                (BlendingFactorDest)AlphaDestinationBlend
            );
        }

        GL.ColorMask(
            WriteChannelEnabled(ColorWriteChannels.Red),
            WriteChannelEnabled(ColorWriteChannels.Green),
            WriteChannelEnabled(ColorWriteChannels.Blue),
            WriteChannelEnabled(ColorWriteChannels.Alpha)
        );
    }

    private bool WriteChannelEnabled(ColorWriteChannels channel)
    {
        return (ColorWriteChannels & channel) == channel;
    }
}