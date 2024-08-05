namespace SmoothGL.Graphics.Shader.Internal;

public abstract class ShaderUniformAssignment<T> : IShaderUniformAssignment
{
    public void ValidateSingle(ShaderUniform uniform, object value)
    {
        if (value is not T)
            throw new ShaderUniformException(
                $"Cannot assign value of type {value.GetType().Name} to uniform {uniform.Name} of type {uniform.Type}.",
                uniform.Name,
                uniform.Type
            );
    }

    public void ValidateArray(ShaderUniform uniform, object value)
    {
        var array = value as T[];

        if (array == null || array.Length != uniform.Size)
        {
            var specificMessage = array switch
            {
                null => $"Value is not an array of type {typeof(T[]).Name}.",
                _ => $"Array size of {array.Length} does not match the required size of {uniform.Size}."
            };

            var message = $"Cannot assign value of type {value.GetType().Name} to uniform {uniform.Name} of type {uniform.Type}. {specificMessage}";
            throw new ShaderUniformException(message, uniform.Name, uniform.Type);
        }
    }

    public virtual void WriteSingleToBuffer(IUnsafeBuffer buffer, object value, int offset)
    {
        buffer.SetData(value, offset);
    }

    public void AssignSingle(int location, object value)
    {
        AssignSingle(location, (T)value);
    }

    public void AssignArray(int location, object value)
    {
        AssignArray(location, (T[])value);
    }

    public virtual bool IsPersistent => true;

    protected abstract void AssignSingle(int location, T value);
    protected abstract void AssignArray(int location, T[] value);
}