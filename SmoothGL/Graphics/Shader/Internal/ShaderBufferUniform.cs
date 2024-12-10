namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderBufferUniform(UniformBufferElement uniformBufferElement, IUnsafeBuffer buffer)
    : ShaderUniform(uniformBufferElement.Name, uniformBufferElement.Type, uniformBufferElement.Size)
{
    protected override void OnValueChanged(object value) =>
        Assignment.WriteToBuffer(buffer, value, uniformBufferElement.Offset);
}