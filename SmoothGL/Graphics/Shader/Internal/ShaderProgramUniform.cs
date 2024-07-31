namespace SmoothGL.Graphics.Internal;

public class ShaderProgramUniform : ShaderUniform
{
    private readonly IShaderUniformAssignment _assignment;
    private readonly IShaderUniformAssignmentDispatcher _dispatcher;
    private readonly int _location;
    private bool _valueHasChanged;

    public ShaderProgramUniform(string name, ShaderUniformType type, int size, int location, IShaderUniformAssignmentDispatcher dispatcher)
        : base(name, type, size)
    {
        _assignment = ShaderUniformAssignmentManager.GetAssignment(type);
        _dispatcher = dispatcher;
        _location = location;
        _valueHasChanged = false;
    }

    public void Apply()
    {
        if (_valueHasChanged || !_assignment.IsPersistent)
        {
            _dispatcher.Assign(_assignment, _location, Value);
            _valueHasChanged = false;
        }
    }

    protected override void OnValueChanged(object value)
    {
        _dispatcher.Validate(_assignment, this, value);
        _valueHasChanged = true;
    }
}