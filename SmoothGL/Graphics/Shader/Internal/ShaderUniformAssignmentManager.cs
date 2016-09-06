using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics.Internal
{
    public class ShaderUniformAssignmentManager
    {
        private static readonly Dictionary<ShaderUniformType, IShaderUniformAssignment> _assignments;
        
        static ShaderUniformAssignmentManager()
        {
            _assignments = new Dictionary<ShaderUniformType, IShaderUniformAssignment>()
            {
                { ShaderUniformType.Bool, new ShaderUniformAssignmentBool() },
                { ShaderUniformType.Double, new ShaderUniformAssignmentDouble() },
                { ShaderUniformType.Double2, new ShaderUniformAssignmentDoubleVector2() },
                { ShaderUniformType.Double3, new ShaderUniformAssignmentDoubleVector3() },
                { ShaderUniformType.Double4, new ShaderUniformAssignmentDoubleVector4() },
                { ShaderUniformType.Float, new ShaderUniformAssignmentFloat() },
                { ShaderUniformType.Float2, new ShaderUniformAssignmentVector2() },
                { ShaderUniformType.Float3, new ShaderUniformAssignmentVector3() },
                { ShaderUniformType.Float4, new ShaderUniformAssignmentVector4() },
                { ShaderUniformType.Int, new ShaderUniformAssignmentInt() },
                { ShaderUniformType.UnsignedInt, new ShaderUniformAssignmentUnsignedInt() },
                { ShaderUniformType.Matrix2, new ShaderUniformAssignmentMatrix2() },
                { ShaderUniformType.Matrix3, new ShaderUniformAssignmentMatrix3() },
                { ShaderUniformType.Matrix4, new ShaderUniformAssignmentMatrix4() },
                { ShaderUniformType.Sampler1D, new ShaderUniformTextureAssignment<Texture1D>() },
                { ShaderUniformType.Sampler2D, new ShaderUniformTextureAssignment<Texture2D>() },
                { ShaderUniformType.Sampler3D, new ShaderUniformTextureAssignment<Texture3D>() },
                { ShaderUniformType.SamplerCube, new ShaderUniformTextureAssignment<TextureCube>() },
            };
        }

        public static IShaderUniformAssignment GetAssignment(ShaderUniformType type)
        {
            IShaderUniformAssignment assignment;
            if (!_assignments.TryGetValue(type, out assignment))
                throw new ArgumentException(String.Format("There is no assignment rule registered for uniforms of type {0}.", type), "type");

            return assignment;
        }
    }
}
