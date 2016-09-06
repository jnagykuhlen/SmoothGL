using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics.Internal
{
    public class ShaderBufferUniform : ShaderUniform
    {
        private IUnsafeBuffer _buffer;
        private IShaderUniformAssignment _assignment;
        private int _offset;

        public ShaderBufferUniform(string name, ShaderUniformType type, int size, IUnsafeBuffer buffer, int offset)
            : base(name, type, size)
        {
            _buffer = buffer;
            _assignment = ShaderUniformAssignmentManager.GetAssignment(type);
            _offset = offset;
        }

        protected override void OnValueChanged(object value)
        {
            _assignment.ValidateSingle(this, value);
            _assignment.WriteSingleToBuffer(_buffer, value, _offset);
        }
    }
}
