namespace SmoothGL.Graphics;

/// <summary>
/// Represents a shader uniform block associated with a single shader program instance.
/// </summary>
public class ShaderUniformBlock
{
    /// <summary>
    /// Creates a new shader uniform block.
    /// </summary>
    /// <param name="name">Name of the uniform block.</param>
    /// <param name="location">Binding point this uniform block uses in the corresponding shader program.</param>
    /// <param name="layout">Layout required for uniform buffers bound to this uniform block.</param>
    public ShaderUniformBlock(string name, int location, UniformBufferLayout layout)
    {
        Name = name;
        Location = location;
        Layout = layout;
        Buffer = null;
    }

    /// <summary>
    /// Gets or sets the uniform buffer which is attached to this shader uniform block.
    /// </summary>
    public UniformBuffer Buffer { get; set; }

    /// <summary>
    /// Gets the name of this uniform block.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the binding point this uniform block uses in the corresponding shader program.
    /// </summary>
    public int Location { get; }

    /// <summary>
    /// Gets the layout required for uniform buffers bound to this uniform block,
    /// defined by the block definition in GLSL shader code.
    /// </summary>
    public UniformBufferLayout Layout { get; }
}