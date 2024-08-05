using OpenTK.Graphics.OpenGL;
using SmoothGL.Graphics.Shader.Internal;

namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Represents a buffer storing a number of uniforms, persistent in graphics memory.
/// </summary>
public class UniformBuffer : Buffer
{
    private readonly Dictionary<string, ShaderUniform> _uniforms;

    /// <summary>
    /// Creates a new structured uniform buffer.
    /// </summary>
    /// <param name="description">Description of the uniforms included in the buffer.</param>
    /// <param name="usage">Hint for the driver concerning the frequency the data in this buffer is expected to change.</param>
    public UniformBuffer(UniformBufferLayout description, BufferUsage usage)
        : base(description.Size, BufferTarget.UniformBuffer, usage)
    {
        _uniforms = description.Elements.Select(CreateUniform).ToDictionary(uniform => uniform.Name);
    }

    /// <summary>
    /// Gets all uniforms contained in this uniform buffer.
    /// </summary>
    public IEnumerable<ShaderUniform> Uniforms => _uniforms.Values;

    protected override string ResourceName => "StructuredUniformBuffer";

    private ShaderUniform CreateUniform(UniformBufferElement element) =>
        new ShaderBufferUniform(element.Name, element.Type, 1, this, element.Offset);

    /// <summary>
    /// Gets the uniform with the specified name contained in this uniform buffer.
    /// Returns null if such uniform does not exist.
    /// </summary>
    /// <param name="name">Name of the uniform.</param>
    /// <returns>Uniform.</returns>
    public ShaderUniform? Uniform(string name) => _uniforms.GetValueOrDefault(name);
}