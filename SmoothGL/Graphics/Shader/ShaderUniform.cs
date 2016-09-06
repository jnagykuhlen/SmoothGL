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
    /// Represents a shader uniform associated with a single shader program instance.
    /// </summary>
    public abstract class ShaderUniform
    {
        private string _name;
        private ShaderUniformType _type;
        private int _size;
        private object _value;
        
        protected ShaderUniform(string name, ShaderUniformType type, int size)
        {
            _name = name;
            _type = type;
            _size = size;
            _value = null;
        }

        protected abstract void OnValueChanged(object value);

        /// <summary>
        /// Gets or sets the value of this uniform in the corresponding shader program. Null values are not allowed.
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Shader uniform value must not be null.");

                OnValueChanged(value);
                _value = value;
            }
        }

        /// <summary>
        /// Gets the number of elements of this uniform in case that it represents an array. Otherwise, 1 is returned.
        /// </summary>
        public int Size
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Gets the name of this uniform.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the type of the value this uniform stores.
        /// </summary>
        public ShaderUniformType Type
        {
            get
            {
                return _type;
            }
        }
    }
}
