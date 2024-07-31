using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
///     Represents possible blending factors in the blending equation.
/// </summary>
public enum Blend
{
    Zero = BlendingFactorSrc.Zero,
    One = BlendingFactorSrc.One,
    SourceColor = BlendingFactorSrc.SrcColor,
    SourceColorInverse = BlendingFactorSrc.OneMinusSrcColor,
    SourceAlpha = BlendingFactorSrc.SrcAlpha,
    SourceAlphaInverse = BlendingFactorSrc.OneMinusSrcAlpha,
    SourceAlphaSaturate = BlendingFactorSrc.SrcAlphaSaturate,
    DestinationColor = BlendingFactorSrc.DstColor,
    DestinationColorInverse = BlendingFactorSrc.OneMinusDstColor,
    DestinationAlpha = BlendingFactorSrc.DstAlpha,
    InverseDestinationAlpha = BlendingFactorSrc.OneMinusDstAlpha
}