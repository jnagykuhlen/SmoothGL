using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics.Internal
{
    public interface IShaderUniformAssignmentDispatcher
    {
        void Validate(IShaderUniformAssignment assignment, ShaderUniform uniform, object value);
        void Assign(IShaderUniformAssignment assignment, int location, object value);
    }
}
