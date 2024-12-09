namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderBufferUniform(string name, ShaderUniformType type, int size, IUnsafeBuffer buffer, int offset)
    : ShaderUniform(name, type, size)
{
    protected override void OnValueChanged(object value)
    {
        Assignment.WriteToBuffer(buffer, value, offset);
    }
}