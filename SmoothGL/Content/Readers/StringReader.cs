using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace SmoothGL.Content
{
    /// <summary>
    /// Reader class which reads a string from a stream.
    /// </summary>
    public class StringReader : IContentReader<string>
    {
        /// <summary>
        /// Reads a string from a stream.
        /// </summary>
        /// <param name="stream">Stream to read data from.</param>
        /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
        /// <param name="contentManager">Content manager used to load additional data.</param>
        /// <returns>The read object.</returns>
        public string Read(Stream stream, Type requestedType, ContentManager contentManager)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Indicates whether this class can also read subtypes of the specified type.
        /// </summary>
        public bool CanReadSubtypes
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of this reader.
        /// </summary>
        public string ReaderName
        {
            get
            {
                return "StringReader";
            }
        }
    }
}
