using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace SmoothGL.Content
{
    /// <summary>
    /// Reader class which deserializes objects of arbitrary type from a stream.
    /// This reader is based on the XmlSerializer class.
    /// </summary>
    public class SerializationReader : IContentReader<object>
    {
        /// <summary>
        /// Deserializes an object from a stream.
        /// </summary>
        /// <param name="stream">Stream to read data from.</param>
        /// <param name="requestedType">The concrete type requested.</param>
        /// <param name="contentManager">Content manager used to load additional data.</param>
        /// <returns>The read object.</returns>
        
        public object Read(Stream stream, Type requestedType, ContentManager contentManager)
        {
            XmlSerializer serializer = new XmlSerializer(requestedType);
            return serializer.Deserialize(stream);
        }

        /// <summary>
        /// Indicates whether this class can also read subtypes of the specified type.
        /// </summary>
        public bool CanReadSubtypes
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the name of this reader.
        /// </summary>
        public string ReaderName
        {
            get
            {
                return "SerializationReader";
            }
        }
    }
}
