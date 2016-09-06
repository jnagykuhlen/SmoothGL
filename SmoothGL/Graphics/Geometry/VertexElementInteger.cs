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
    /// Describes a vertex element as a vector of integers.
    /// </summary>
    public class VertexElementInteger : IVertexElement
    {
        private static readonly Dictionary<IntegerSourceType, int> _sourceTypeSizes;

        private int _location;
        private int _numberOfComponents;
        private IntegerSourceType _sourceType;
        
        static VertexElementInteger()
        {
            _sourceTypeSizes = new Dictionary<IntegerSourceType, int>(6);
            _sourceTypeSizes.Add(IntegerSourceType.Byte, 1);
            _sourceTypeSizes.Add(IntegerSourceType.Short, 2);
            _sourceTypeSizes.Add(IntegerSourceType.Int, 4);
            _sourceTypeSizes.Add(IntegerSourceType.UnsignedByte, 1);
            _sourceTypeSizes.Add(IntegerSourceType.UnsignedShort, 2);
            _sourceTypeSizes.Add(IntegerSourceType.UnsignedInt, 4);
        }

        /// <summary>
        /// Creates a new vertex element description for a vector of 32-bit signed integers.
        /// </summary>
        /// <param name="location">Location at which this vertex element is accessible in the vertex shader.</param>
        /// <param name="numberOfComponents">Number of components of the described vector in the range between one and four.</param>
        public VertexElementInteger(int location, int numberOfComponents)
            : this(location, numberOfComponents, IntegerSourceType.Int)
        {
        }

        /// <summary>
        /// Creates a new vertex element description for a vector of integers.
        /// </summary>
        /// <param name="location">Location at which this vertex element is accessible in the vertex shader.</param>
        /// <param name="numberOfComponents">Number of components of the described vector in the range between one and four.</param>
        /// <param name="sourceType">Concrete data type stored.</param>
        public VertexElementInteger(int location, int numberOfComponents, IntegerSourceType sourceType)
        {
            _location = location;
            _numberOfComponents = numberOfComponents;
            _sourceType = sourceType;
        }

        /// <summary>
        /// Commmunicates this vertex element definition to the GPU. This method is not required to be called by client code.
        /// </summary>
        /// <param name="strideSize">The length of a single vertex representation in memory, in bytes.</param>
        /// <param name="offset">The offset at which this element is placed, in bytes.</param>
        /// <param name="divisor">The divisor.</param>
        public void ApplyDefinition(int strideSize, int offset, int divisor)
        {
            GL.EnableVertexAttribArray(_location);
            GL.VertexAttribIPointer(_location, _numberOfComponents, (VertexAttribIntegerType)_sourceType, strideSize, new IntPtr(offset));
            GL.VertexAttribDivisor(_location, divisor);
        }

        /// <summary>
        /// Gets the number of bytes required to represent this vertex element in memory.
        /// </summary>
        public int Size
        {
            get
            {
                return _numberOfComponents * _sourceTypeSizes[_sourceType];
            }
        }
    }
}
