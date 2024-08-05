using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderUniformTextureAssignment<T> : ShaderUniformAssignment<T>
    where T : Texture
{
    public override bool IsPersistent => false;

    protected override void AssignSingle(int location, T value)
    {
        GL.ActiveTexture(TextureUnit.Texture0 + location);
        value.Bind();
    }

    protected override void AssignArray(int location, T[] value)
    {
        for (var i = 0; i < value.Length; ++i)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + location + i);
            value[i].Bind();
        }
    }

    public override void WriteSingleToBuffer(IUnsafeBuffer buffer, object value, int offset) =>
        throw new InvalidOperationException("Texture uniforms cannot be written to buffers.");
}