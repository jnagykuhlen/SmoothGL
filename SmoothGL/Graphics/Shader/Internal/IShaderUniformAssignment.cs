using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics.Internal
{
    public interface IShaderUniformAssignment
    {
        void ValidateSingle(ShaderUniform uniform, object value);
        void ValidateArray(ShaderUniform uniform, object value);
        void AssignSingle(int location, object value);
        void AssignArray(int location, object value);
        void WriteSingleToBuffer(IUnsafeBuffer buffer, object value, int offset);
        bool IsPersistent { get; }
    }
}
