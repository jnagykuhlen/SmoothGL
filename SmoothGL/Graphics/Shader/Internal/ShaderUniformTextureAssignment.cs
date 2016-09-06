using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics.Internal
{
    public class ShaderUniformTextureAssignment<T> : ShaderUniformAssignment<T>
        where T : Texture
    {
        protected override void AssignSingle(int location, T value)
        {
            if (value != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + location);
                ((Texture)value).Bind();
            }
        }

        protected override void AssignArray(int location, T[] value)
        {
            if (value != null)
            {
                Texture[] textures = (Texture[])value;

                for (int i = 0; i < textures.Length; ++i)
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

        public override bool IsPersistent
        {
            get
            {
                return false;
            }
        }
    }
}
