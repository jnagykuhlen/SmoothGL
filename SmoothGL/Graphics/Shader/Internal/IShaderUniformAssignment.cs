namespace SmoothGL.Graphics.Shader.Internal;

public interface IShaderUniformAssignment
{
    bool IsPersistent { get; }
    void ValidateSingle(ShaderUniform uniform, object value);
    void ValidateArray(ShaderUniform uniform, object value);
    void AssignSingle(int location, object value);
    void AssignArray(int location, object value);
    void WriteSingleToBuffer(IUnsafeBuffer buffer, object value, int offset);
}