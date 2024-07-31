namespace SmoothGL.Graphics.Internal;

public abstract class ShaderUniformAssignment<T> : IShaderUniformAssignment
{
    public void ValidateSingle(ShaderUniform uniform, object value)
    {
        if (!(value is T))
            throw new ShaderUniformException(
                string.Format("Cannot assign value of type {0} to uniform {1} of type {2}.", value.GetType().Name, uniform.Name, uniform.Type),
                uniform.Name,
                uniform.Type
            );
    }

    public void ValidateArray(ShaderUniform uniform, object value)
    {
        var array = value as T[];

        if (array == null || array.Length != uniform.Size)
        {
            var baseMessage = string.Format
            (
                "Cannot assign value of type {0} to uniform {1} of type {2}. ",
                value.GetType().Name,
                uniform.Name,
                uniform.Type
            );

            string specificMessage;
            if (array == null)
                specificMessage = string.Format("Value is not an array of type {0}.", typeof(T[]).Name);
            else
                specificMessage = string.Format("Array size of {0} does not match the required size of {1}.", array.Length, uniform.Size);

            throw new ShaderUniformException(baseMessage + specificMessage, uniform.Name, uniform.Type);
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