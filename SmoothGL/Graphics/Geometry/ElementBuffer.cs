using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a buffer storing index data, persistent in graphics memory.
    /// </summary>
    public class ElementBuffer : Buffer
    {
        private int _numberOfElements;
        private ElementType _elementType;
        
        /// <summary>
        /// Creates a new element buffer.
        /// </summary>
        /// <param name="numberOfElements">Maximum number of indices stored in this buffer.</param>
        /// <param name="elementType">Specifies the integer data type used for indices.</param>
        /// <param name="usage">Hint for the driver concerning the frequency the data in this buffer is expected to change.</param>
        public ElementBuffer(int numberOfElements, ElementType elementType, BufferUsage usage)
            : base(numberOfElements * GetElementTypeSize(elementType), BufferTarget.ElementArrayBuffer, usage)
        {
            _numberOfElements = numberOfElements;
            _elementType = elementType;
        }

        private static int GetElementTypeSize(ElementType elementType)
        {
            switch(elementType)
            {
                case ElementType.UnsignedByte:
                    return sizeof(byte);
                case ElementType.UnsignedShort:
                    return sizeof(ushort);
                case ElementType.UnsignedInt:
                    return sizeof(uint);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Resizes this buffer in a way that it can store the specified number of indices. All index data in this buffer will be discarded.
        /// </summary>
        /// <param name="numberOfElements">Maximum number of indices stored in this buffer.</param>
        public new void Resize(int numberOfElements)
        {
            _numberOfElements = numberOfElements;
            base.Resize(numberOfElements * GetElementTypeSize(_elementType));
        }

        private void SetData<T>(T[] data, ElementType requestedElementType)
            where T : struct
        {
            if (_elementType != requestedElementType)
                throw new ArgumentException(string.Format("Element buffer expects indices of type {0} instead of {1}.", _elementType, requestedElementType));

            if (data.Length > _numberOfElements)
                throw new ArgumentException("Cannot set data that exceeds buffer size.");

            SetData(data, 0, data.Length * GetElementTypeSize(_elementType));
        }

        /// <summary>
        /// Uploads index data to the GPU. An ArgumentException is thrown if the index data type of this buffer is not UnsignedByte.
        /// </summary>
        /// <param name="data">Data to upload to this buffer.</param>
        public void SetData(byte[] data)
        {
            SetData(data, ElementType.UnsignedByte);
        }

        /// <summary>
        /// Uploads index data to the GPU. An ArgumentException is thrown if the index data type of this buffer is not UnsignedShort.
        /// </summary>
        /// <param name="data">Data to upload to this buffer.</param>
        public void SetData(ushort[] data)
        {
            SetData(data, ElementType.UnsignedShort);
        }

        /// <summary>
        /// Uploads index data to the GPU. An ArgumentException is thrown if the index data type of this buffer is not UnsignedInt.
        /// </summary>
        /// <param name="data">Data to upload to this buffer.</param>
        public void SetData(uint[] data)
        {
            SetData(data, ElementType.UnsignedInt);
        }

        protected override string ResourceName
        {
            get
            {
                return "ElementBuffer";
            }
        }

        /// <summary>
        /// Gets the integer data type used for indices in this buffer.
        /// </summary>
        public ElementType ElementType
        {
            get
            {
                return _elementType;
            }
        }

        /// <summary>
        /// Gets the maximum number of elements which can be stored in this buffer.
        /// </summary>
        public int NumberOfElements
        {
            get
            {
                return _numberOfElements;
            }
        }

        /// <summary>
        /// Gets the size of a single element stored in this buffer, measured in bytes.
        /// </summary>
        public int ElementSize
        {
            get
            {
                return GetElementTypeSize(_elementType);
            }
        }
    }
}
