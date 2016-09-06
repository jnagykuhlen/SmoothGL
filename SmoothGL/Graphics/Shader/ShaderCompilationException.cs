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
    /// Exception which is thrown when a shader fails to compile.
    /// </summary>
    public class ShaderCompilationException : Exception
    {
        private ShaderType _shaderType;
        private string _shaderCode;

        /// <summary>
        /// Creates a new ShaderCompilationException.
        /// </summary>
        /// <param name="message">Message specifying why the shader does not compile.</param>
        /// <param name="shaderType">Type of the shader which does not compile.</param>
        public ShaderCompilationException(string message, ShaderType shaderType)
            : this(message, shaderType, null) { }

        /// <summary>
        /// Creates a new ShaderCompilationException.
        /// </summary>
        /// <param name="message">Message specifying why the shader does not compile.</param>
        /// <param name="shaderType">Type of the shader which does not compile.</param>
        /// <param name="shaderCode">Incorrect shader code causing the compilation error.</param>
        public ShaderCompilationException(string message, ShaderType shaderType, string shaderCode)
            : base(message)
        {
            _shaderType = shaderType;
            _shaderCode = shaderCode;
        }

        /// <summary>
        /// Gets the type of the shader which does not compile.
        /// </summary>
        public ShaderType ShaderType
        {
            get
            {
                return _shaderType;
            }
        }

        /// <summary>
        /// Gets the incorrect shader code causing the compilation error.
        /// </summary>
        public string ShaderCode
        {
            get
            {
                return _shaderCode;
            }
        }

    }
}
