namespace SmoothGL.Graphics;

/// <summary>
///     Describes a single uniform in a uniform buffer.
/// </summary>
public class UniformBufferElement
{
    /// <summary>
    ///     Creates a new description of a uniform in a uniform buffer.
    /// </summary>
    /// <param name="name">Name of the uniform.</param>
    /// <param name="type">Type of the value the uniform stores.</param>
    /// <param name="offset">Offset of this uniform relative to the start of the containing uniform buffer, in bytes.</param>
    public UniformBufferElement(string name, ShaderUniformType type, int offset)
    {
        if (type == ShaderUniformType.Sampler1D ||
            type == ShaderUniformType.Sampler2D ||
            type == ShaderUniformType.Sampler3D ||
            type == ShaderUniformType.SamplerCube)
            throw new ArgumentException("Texture samplers are not allowed to be defined in uniform blocks.");

        if (type == ShaderUniformType.Double ||
            type == ShaderUniformType.Double2 ||
            type == ShaderUniformType.Double3 ||
            type == ShaderUniformType.Double4 ||
            type == ShaderUniformType.Matrix3)
            throw new ShaderUniformException(
                string.Format("The uniform type {0} is currently not supported for uniform buffer elements.", type),
                name,
                type
            );

        if (offset < 0)
            throw new ArgumentOutOfRangeException("offset", "Offset must not be negative.");

        Name = name;
        Type = type;
        Offset = offset;
    }

    /// <summary>
    ///     Gets the name of the uniform.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the type of the value the uniform stores.
    /// </summary>
    public ShaderUniformType Type { get; }

    /// <summary>
    ///     Gets the offset of this uniform relative to the start of the containing uniform buffer, in bytes.
    /// </summary>
    public int Offset { get; }
}