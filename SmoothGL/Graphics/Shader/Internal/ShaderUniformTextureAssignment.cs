using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Internal;

public class ShaderUniformTextureAssignment<T> : ShaderUniformAssignment<T>
    where T : Texture
{
    public override bool IsPersistent => false;

    protected override void AssignSingle(int location, T value)
    {
        if (value != null)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + location);
            value.Bind();
        }
    }

    protected override void AssignArray(int location, T[] value)
    {
        if (value != null)
        {
            Texture[] textures = value;

            for (var i = 0; i < textures.Length; ++i)
            {
                if (textures[i] == null)
                    throw new InvalidCastException("Texture uniform arrays are not allowed to contain null values.");

                GL.ActiveTexture(TextureUnit.Texture0 + location + i);
                textures[i].Bind();
            }
        }
    }

    public override void WriteSingleToBuffer(IUnsafeBuffer buffer, object value, int offset)
    {
        throw new InvalidOperationException("Texture uniforms cannot be written to buffers.");
    }
}