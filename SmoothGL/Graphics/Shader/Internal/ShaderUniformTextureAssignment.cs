using OpenTK.Graphics.OpenGL;
using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderUniformTextureAssignment(Func<object, bool> validationFunction) : IShaderUniformAssignment
{
    public bool Validate(object value) => validationFunction(value);

    public void Assign(int location, object value)
    {
        GL.ActiveTexture(TextureUnit.Texture0 + location);
        ((Texture)value).Bind();
    }

    public void WriteToBuffer(IUnsafeBuffer buffer, object value, int offset) =>
        throw new InvalidOperationException("Texture uniforms cannot be written to buffers.");

    public bool IsPersistent => false;
}