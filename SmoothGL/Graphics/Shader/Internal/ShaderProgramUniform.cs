namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderProgramUniform(string name, ShaderUniformType type, int size, int location, IShaderUniformAssignmentDispatcher dispatcher)
    : ShaderUniform(name, type, size)
{
    private readonly IShaderUniformAssignment _assignment = ShaderUniformAssignmentManager.GetAssignment(type);
    private bool _valueHasChanged;

    public void Apply()
    {
        if (_valueHasChanged || !_assignment.IsPersistent)
        {
            dispatcher.Assign(_assignment, location, Value);
            _valueHasChanged = false;
        }
    }

    protected override void OnValueChanged(object value)
    {
        dispatcher.Validate(_assignment, this, value);
        _valueHasChanged = true;
    }
}