namespace SmoothGL.Graphics.Internal;

public class ShaderBufferUniform : ShaderUniform
{
    private readonly IShaderUniformAssignment _assignment;
    private readonly IUnsafeBuffer _buffer;
    private readonly int _offset;

    public ShaderBufferUniform(string name, ShaderUniformType type, int size, IUnsafeBuffer buffer, int offset)
        : base(name, type, size)
    {
        _buffer = buffer;
        _assignment = ShaderUniformAssignmentManager.GetAssignment(type);
        _offset = offset;
    }

    protected override void OnValueChanged(object value)
    {
        _assignment.ValidateSingle(this, value);
        _assignment.WriteSingleToBuffer(_buffer, value, _offset);
    }
}