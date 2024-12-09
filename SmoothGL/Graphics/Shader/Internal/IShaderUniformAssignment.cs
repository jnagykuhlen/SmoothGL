namespace SmoothGL.Graphics.Shader.Internal;

public interface IShaderUniformAssignment
{
    bool Validate(object value);
    void Assign(int location, object value);
    void WriteToBuffer(IUnsafeBuffer buffer, object value, int offset);
    bool IsPersistent { get; }
}