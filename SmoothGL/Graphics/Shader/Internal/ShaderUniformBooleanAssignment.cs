using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderUniformBooleanAssignment : IShaderUniformAssignment
{
    public bool Validate(object value) => value is bool;
    public void Assign(int location, object value) => GL.Uniform1(location, ToInt(value));
    public void WriteToBuffer(IUnsafeBuffer buffer, object value, int offset) => buffer.SetData(ToInt(value), offset);
    public bool IsPersistent => true;

    private static int ToInt(object value) => (bool)value ? 1 : 0;
}