using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Content
{
    /// <summary>
    /// Exception which is thrown when content cannot be read by the content manager.
    /// </summary>
    public class ContentLoadException : Exception
    {
        private string _filename;
        private Type _contentType;

        /// <summary>
        /// Creates a new ContentLoadException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception which indicated that the read operation failed.</param>
        /// <param name="filename">Path to the content file for which loading failed.</param>
        /// <param name="contentType">Requested content type for which loading failed.</param>
        public ContentLoadException(string message, Exception innerException, string filename, Type contentType)
            : base(message, innerException)
        {
            _filename = filename;
            _contentType = contentType;
        }

        /// <summary>
        /// Creates a new ContentLoadException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="filename">Path to the content file for which loading failed.</param>
        /// <param name="contentType">Requested content type for which loading failed.</param>
        public ContentLoadException(string message, string filename, Type contentType)
            : this(message, null, filename, contentType)
        {
        }

        /// <summary>
        /// Gets the path to the content file for which loading failed.
        /// </summary>
        public string Filename
        {
            get
            {
                return _filename;
            }
        }

        /// <summary>
        /// Gets the requested content type for which loading failed.
        /// </summary>
        public Type ContentType
        {
            get
            {
                return _contentType;
            }
        }
    }
}
