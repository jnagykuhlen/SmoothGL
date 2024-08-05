namespace SmoothGL.Graphics.Texturing;

/// <summary>
/// Determines how color values are interpolated when sampling a texture between
/// its grid points.
/// </summary>
public enum TextureInterpolation
{
    /// <summary>
    /// The resulting color value is the value of the nearest grid point in the texture.
    /// </summary>
    Nearest,

    /// <summary>
    /// The resulting color is the weighted, linear interpolation of the four
    /// surrounding grid points in the texture.
    /// </summary>
    Linear
}