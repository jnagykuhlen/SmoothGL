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
    /// <param name="size">Number of elements of this uniform in case that it represents an array, 1 otherwise.</param>
    /// <param name="offset">Offset of this uniform relative to the start of the containing uniform buffer, in bytes.</param>
    public UniformBufferElement(string name, ShaderUniformType type, int size, int offset)
    {
        if (type.IsSampler())
            throw new ArgumentException("Texture samplers are not allowed to be defined in uniform blocks.", nameof(type));

        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset must not be negative.");

        Name = name;
        Type = type;
        Size = size;
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
    /// Gets the number of elements of this uniform in case that it represents an array. Otherwise, 1 is returned.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets the offset of this uniform relative to the start of the containing uniform buffer, in bytes.
    /// </summary>
    public int Offset { get; }
}