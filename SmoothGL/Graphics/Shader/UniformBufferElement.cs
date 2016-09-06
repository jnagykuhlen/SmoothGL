using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Describes a single uniform in a uniform buffer.
    /// </summary>
    public class UniformBufferElement
    {
        private string _name;
        private ShaderUniformType _type;
        private int _offset;

        /// <summary>
        /// Creates a new description of a uniform in a uniform buffer.
        /// </summary>
        /// <param name="name">Name of the uniform.</param>
        /// <param name="type">Type of the value the uniform stores.</param>
        /// <param name="offset">Offset of this uniform relative to the start of the containing uniform buffer, in bytes.</param>
        public UniformBufferElement(string name, ShaderUniformType type, int offset)
        {
            if (type == ShaderUniformType.Sampler1D ||
                type == ShaderUniformType.Sampler2D ||
                type == ShaderUniformType.Sampler3D ||
                type == ShaderUniformType.SamplerCube)
            {
                throw new ArgumentException("Texture samplers are not allowed to be defined in uniform blocks.");
            }

            if(type == ShaderUniformType.Double ||
               type == ShaderUniformType.Double2 || 
               type == ShaderUniformType.Double3 ||
               type == ShaderUniformType.Double4 ||
               type == ShaderUniformType.Matrix3)
            {
                throw new ShaderUniformException(
                    String.Format("The uniform type {0} is currently not supported for uniform buffer elements.", type),
                    name,
                    type
                );
            }

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Offset must not be negative.");

            _name = name;
            _type = type;
            _offset = offset;
        }

        /// <summary>
        /// Gets the name of the uniform.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the type of the value the uniform stores.
        /// </summary>
        public ShaderUniformType Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// Gets the offset of this uniform relative to the start of the containing uniform buffer, in bytes.
        /// </summary>
        public int Offset
        {
            get
            {
                return _offset;
            }
        }
    }
}
