namespace SmoothGL.Graphics.State;

/// <summary>
/// Defines which color channels of the current framebuffer are affected by blending operations.
/// </summary>
[Flags]
public enum ColorWriteChannels
{
    None = 0x00,
    Red = 0x01,
    Green = 0x02,
    Blue = 0x04,
    Alpha = 0x08,
    All = Red | Green | Blue | Alpha
}