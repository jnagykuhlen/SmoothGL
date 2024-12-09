namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderProgramUniform(string name, ShaderUniformType type, int size, int location)
    : ShaderUniform(name, type, size)
{
    private bool _valueHasChanged;

    public void Apply()
    {
        if (_valueHasChanged || !Assignment.IsPersistent)
        {
            var value = Value ?? throw new ShaderUniformException($"Value of shader uniform {Name} has not been set yet.", Name, Type);
            Assignment.Assign(location, value);
            _valueHasChanged = false;
        }
    }

    protected override void OnValueChanged(object value)
    {
        _valueHasChanged = true;
    }
}