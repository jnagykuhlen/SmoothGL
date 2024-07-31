namespace SmoothGL.Graphics;

/// <summary>
/// Represents a shader uniform associated with a single shader program instance.
/// </summary>
public abstract class ShaderUniform
{
    private object _value;

    protected ShaderUniform(string name, ShaderUniformType type, int size)
    {
        Name = name;
        Type = type;
        Size = size;
        _value = null;
    }

    /// <summary>
    /// Gets or sets the value of this uniform in the corresponding shader program. Null values are not allowed.
    /// </summary>
    public object Value
    {
        get => _value;
        set
        {
            if (value == null)
                throw new ArgumentNullException("value", "Shader uniform value must not be null.");

            OnValueChanged(value);
            _value = value;
        }
    }

    /// <summary>
    /// Gets the number of elements of this uniform in case that it represents an array. Otherwise, 1 is returned.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets the name of this uniform.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the value this uniform stores.
    /// </summary>
    public ShaderUniformType Type { get; }

    protected abstract void OnValueChanged(object value);
}