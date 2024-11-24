namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Represents a shader uniform associated with a single shader program instance.
/// </summary>
public abstract class ShaderUniform(string name, ShaderUniformType type, int size)
{
    /// <summary>
    /// Gets the value of this uniform in the corresponding shader program.
    /// </summary>
    public object? Value { get; private set; }

    /// <summary>
    /// Sets the value of this uniform in the corresponding shader program.
    /// </summary>
    public void SetValue(object value)
    {
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
}