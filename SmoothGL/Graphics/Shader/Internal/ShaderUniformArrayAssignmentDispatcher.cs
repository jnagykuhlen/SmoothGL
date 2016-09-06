using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics.Internal
{
    public class ShaderUniformArrayAssignmentDispatcher : IShaderUniformAssignmentDispatcher
    {
        public static readonly ShaderUniformArrayAssignmentDispatcher Instance = new ShaderUniformArrayAssignmentDispatcher();

        public void Assign(IShaderUniformAssignment assignment, int location, object value)
        {
            assignment.AssignArray(location, value);
        }

        public void Validate(IShaderUniformAssignment assignment, ShaderUniform uniform, object value)
        {
            assignment.ValidateArray(uniform, value);
        }
    }
}
