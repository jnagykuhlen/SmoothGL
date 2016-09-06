using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SmoothGL.Graphics.Internal;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Describes how the uniforms included in a uniform buffer are arranged in memory.
    /// </summary>
    public class UniformBufferLayout
    {
        private UniformBufferElement[] _elements;
        private int _size;

        /// <summary>
        /// Creates a new uniform buffer layout, including the specified elements.
        /// </summary>
        /// <param name="size">Size in bytes required to store the specified uniforms in a uniform buffer.</param>
        /// <param name="elements">Uniforms included in the uniform buffer layout.</param>
        public UniformBufferLayout(int size, params UniformBufferElement[] elements)
        {
            _size = size;
            _elements = elements;
        }
        
        /// <summary>
        /// Gets the uniforms included in this uniform buffer layout.
        /// </summary>
        public UniformBufferElement[] Elements
        {
            get
            {
                return _elements;
            }
        }

        /// <summary>
        /// Gets the number of bytes required to store the specified uniforms in a uniform buffer.
        /// </summary>
        public int Size
        {
            get
            {
                return _size;
            }
        }
    }
}
