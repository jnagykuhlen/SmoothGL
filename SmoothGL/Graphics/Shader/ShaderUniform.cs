using SmoothGL.Graphics.Shader.Internal;

namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Represents a shader uniform associated with a single shader program instance.
/// </summary>
public abstract class ShaderUniform(string name, ShaderUniformType type, int size)
{
    protected IShaderUniformAssignment Assignment { get; } = ShaderUniformAssignments.Get(type, size);
    
    /// <summary>
    /// Gets the value of this uniform in the corresponding shader program.
    /// </summary>
    public object? Value { get; private set; }

    /// <summary>
    /// Sets the value of this uniform in the corresponding shader program.
    /// </summary>
    public void SetValue(object value)
    {
        if (!Assignment.Validate(value))
            throw new ShaderUniformException($"Cannot assign value of type {value.GetType().Name} to uniform {Name} of type {FormattedType}.", Name, Type);
        
        OnValueChanged(value);
        Value = value;
    }

    /// <summary>
    /// Gets the number of elements of this uniform in case that it represents an array. Otherwise, 1 is returned.
    /// </summary>
    public int Size { get; } = size;

    /// <summary>
    /// Gets the name of this uniform.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the type of the value this uniform stores.
    /// </summary>
    public ShaderUniformType Type { get; } = type;

    protected abstract void OnValueChanged(object value);
    
    private string FormattedType => Size == 1 ? Type.ToString() : $"{Type}[{Size}]";
}