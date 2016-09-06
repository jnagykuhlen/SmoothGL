using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SmoothGL.Graphics.Internal;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a buffer storing a number of uniforms, persistent in graphics memory.
    /// </summary>
    public class UniformBuffer : Buffer
    {
        private Dictionary<string, ShaderUniform> _uniforms;
        
        /// <summary>
        /// Creates a new structured uniform buffer.
        /// </summary>
        /// <param name="description">Description of the uniforms included in the buffer.</param>
        /// <param name="usage">Hint for the driver concerning the frequency the data in this buffer is expected to change.</param>
        public UniformBuffer(UniformBufferLayout description, BufferUsage usage)
            : base(description.Size, BufferTarget.UniformBuffer, usage)
        {
            _uniforms = description.Elements.Select(CreateUniform).ToDictionary(uniform => uniform.Name);
        }

        private ShaderUniform CreateUniform(UniformBufferElement element)
        {
            IShaderUniformAssignment assignment = ShaderUniformAssignmentManager.GetAssignment(element.Type);
            return new ShaderBufferUniform(element.Name, element.Type, 1, this, element.Offset);
        }

        /// <summary>
        /// Gets the uniform with the specified name contained in this uniform buffer.
        /// Returns null if such uniform does not exist.
        /// </summary>
        /// <param name="name">Name of the uniform.</param>
        /// <returns>Uniform.</returns>
        public ShaderUniform Uniform(string name)
        {
            ShaderUniform result;
            if (_uniforms.TryGetValue(name, out result))
                return result;
            return null;
        }

        /// <summary>
        /// Gets all uniforms contained in this uniform buffer.
        /// </summary>
        public IEnumerable<ShaderUniform> Uniforms
        {
            get
            {
                return _uniforms.Values;
            }
        }

        protected override string ResourceName
        {
            get
            {
                return "StructuredUniformBuffer";
            }
        }
    }
}
