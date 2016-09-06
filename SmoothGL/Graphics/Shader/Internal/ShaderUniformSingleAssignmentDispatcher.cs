using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics.Internal
{
    public class ShaderUniformSingleAssignmentDispatcher : IShaderUniformAssignmentDispatcher
    {
        public static readonly ShaderUniformSingleAssignmentDispatcher Instance = new ShaderUniformSingleAssignmentDispatcher();

        public void Assign(IShaderUniformAssignment assignment, int location, object value)
        {
            assignment.AssignSingle(location, value);
        }

        public void Validate(IShaderUniformAssignment assignment, ShaderUniform uniform, object value)
        {
            assignment.ValidateSingle(uniform, value);
        }
    }
}
