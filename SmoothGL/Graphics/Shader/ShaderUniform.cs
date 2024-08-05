namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Represents a shader uniform associated with a single shader program instance.
/// </summary>
public abstract class ShaderUniform(string name, ShaderUniformType type, int size)
{
    private object? _value;

    /// <summary>
    /// Gets or sets the value of this uniform in the corresponding shader program.
    /// </summary>
    public object Value
    {
        get => _value ?? throw new InvalidOperationException("Shader uniform value has not been set yet.");
        set
        {
            OnValueChanged(value);
            _value = value;
        }
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
}