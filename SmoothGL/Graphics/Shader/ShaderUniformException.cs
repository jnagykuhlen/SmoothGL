using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Exception which is thrown when an invalid operation is performed on shader uniforms.
    /// </summary>
    public class ShaderUniformException : Exception
    {
        private string _uniformName;
        private ShaderUniformType _uniformType;

        /// <summary>
        /// Creates a new ShaderUniformException.
        /// </summary>
        /// <param name="message">Message specifying the reason for the exception.</param>
        /// <param name="uniformName">Name of the shader uniform which caused the exception.</param>
        /// <param name="uniformType">Type of the shader uniform which caused the exception.</param>
        public ShaderUniformException(string message, string uniformName, ShaderUniformType uniformType)
            : base(message)
        {
            _uniformName = uniformName;
            _uniformType = uniformType;
        }

        /// <summary>
        /// Gets the name of the shader uniform which caused the exception.
        /// </summary>
        public string UniformName
        {
            get
            {
                return _uniformName;
            }
        }

        /// <summary>
        /// Gets the type of the shader uniform which caused the exception.
        /// </summary>
        public ShaderUniformType UniformType
        {
            get
            {
                return _uniformType;
            }
        }
    }
}
