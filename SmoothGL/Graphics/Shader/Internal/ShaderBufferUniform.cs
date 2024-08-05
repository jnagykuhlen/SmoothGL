namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderBufferUniform(string name, ShaderUniformType type, int size, IUnsafeBuffer buffer, int offset)
    : ShaderUniform(name, type, size)
{
    private readonly IShaderUniformAssignment _assignment = ShaderUniformAssignmentManager.GetAssignment(type);

    protected override void OnValueChanged(object value)
    {
        _assignment.ValidateSingle(this, value);
        _assignment.WriteSingleToBuffer(buffer, value, offset);
    }
}