namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderUniformAssignment<T>(Action<int, T> assignmentAction) : IShaderUniformAssignment where T : struct
{
    public bool Validate(object value) => value is T;
    public void Assign(int location, object value) => assignmentAction(location, (T)value);
    public void WriteToBuffer(IUnsafeBuffer buffer, object value, int offset) => buffer.SetData(value, offset);
    public bool IsPersistent => true;
}