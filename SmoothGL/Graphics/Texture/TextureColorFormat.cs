using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Specifies the format of color values stored in a texture. Possible color components
/// are red, green, blue and alpha denoted by their first letter. The number indicates how
/// many bits are reserved for the full color value, i.e., for the sum of its components.
/// When specified as float, component values outside the range between 0.0 and 1.0 are not
/// clamped to this interval but stored with floating-point precision.
/// </summary>
public enum TextureColorFormat
{
    Rgba16 = PixelInternalFormat.Rgba4,
    Rgba32 = PixelInternalFormat.Rgba,
    Rgba64 = PixelInternalFormat.Rgba16,
    Rgba64float = PixelInternalFormat.Rgba16f,
    Rgba128float = PixelInternalFormat.Rgba32f,
    R8 = PixelInternalFormat.R8,
    R16 = PixelInternalFormat.R16,
    R16float = PixelInternalFormat.R16f,
    R32float = PixelInternalFormat.R32f,
    Rg16 = PixelInternalFormat.Rg8,
    Rg32 = PixelInternalFormat.Rg16,
    Rg32float = PixelInternalFormat.Rg16f,
    Rg64float = PixelInternalFormat.Rg32f
}