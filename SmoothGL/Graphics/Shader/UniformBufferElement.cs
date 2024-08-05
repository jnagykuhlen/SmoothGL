namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Describes a single uniform in a uniform buffer.
/// </summary>
public class UniformBufferElement
{
    /// <summary>
    /// Creates a new description of a uniform in a uniform buffer.
    /// </summary>
    /// <param name="name">Name of the uniform.</param>
    /// <param name="type">Type of the value the uniform stores.</param>
    /// <param name="offset">Offset of this uniform relative to the start of the containing uniform buffer, in bytes.</param>
    public UniformBufferElement(string name, ShaderUniformType type, int offset)
    {
        if (type is
            ShaderUniformType.Sampler1D or
            ShaderUniformType.Sampler2D or
            ShaderUniformType.Sampler3D or
            ShaderUniformType.SamplerCube)
        {
            throw new ArgumentException("Texture samplers are not allowed to be defined in uniform blocks.", nameof(type));
        }

        if (type is
            ShaderUniformType.Double or
            ShaderUniformType.Double2 or
            ShaderUniformType.Double3 or
            ShaderUniformType.Double4 or
            ShaderUniformType.Matrix3)
        {
            throw new ShaderUniformException(
                $"The uniform type {type} is currently not supported for uniform buffer elements.",
                name,
                type
            );
        }

        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset must not be negative.");

        Name = name;
        Type = type;
        Offset = offset;
    }

    /// <summary>
    /// Gets the name of the uniform.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the value the uniform stores.
    /// </summary>
    public ShaderUniformType Type { get; }

    /// <summary>
    /// Gets the offset of this uniform relative to the start of the containing uniform buffer, in bytes.
    /// </summary>
    public int Offset { get; }
}