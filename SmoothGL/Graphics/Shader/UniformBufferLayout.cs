namespace SmoothGL.Graphics;

/// <summary>
/// Describes how the uniforms included in a uniform buffer are arranged in memory.
/// </summary>
public class UniformBufferLayout
{
    /// <summary>
    /// Creates a new uniform buffer layout, including the specified elements.
    /// </summary>
    /// <param name="size">Size in bytes required to store the specified uniforms in a uniform buffer.</param>
    /// <param name="elements">Uniforms included in the uniform buffer layout.</param>
    public UniformBufferLayout(int size, params UniformBufferElement[] elements)
    {
        Size = size;
        Elements = elements;
    }

    /// <summary>
    /// Gets the uniforms included in this uniform buffer layout.
    /// </summary>
    public UniformBufferElement[] Elements { get; }

    /// <summary>
    /// Gets the number of bytes required to store the specified uniforms in a uniform buffer.
    /// </summary>
    public int Size { get; }
}