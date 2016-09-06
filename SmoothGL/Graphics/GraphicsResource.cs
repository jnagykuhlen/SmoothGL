using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics
{
    /// <summary>
    /// Defines an abstract resource in graphics memory.
    /// </summary>
    public abstract class GraphicsResource : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Creates a new graphics resource.
        /// </summary>
        protected GraphicsResource()
        {
            _disposed = false;
        }

        /// <summary>
        /// Disposes this resource, freeing all allocated graphics memory.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                FreeResources();
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        /// <summary>
        /// Throws an ObjectDisposedException in case this resource is already disposed.
        /// </summary>
        protected void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(ResourceName, "The object is already disposed.");
        }

        /// <summary>
        /// Override this method to free unmanaged resources.
        /// </summary>
        protected virtual void FreeResources()
        {
        }

        /// <summary>
        /// Override this method to return the name of the resource.
        /// </summary>
        protected virtual string ResourceName
        {
            get
            {
                return "GraphicsResource";
            }
        }

        ~GraphicsResource()
        {
            if (!_disposed)
            {
                try
                {
                    Dispose();
                }
                catch (Exception exception)
                {
                    throw new GraphicsResourceNotDisposedException(
                        string.Format(
                            "{0} has not been disposed before finalization and cannot be disposed automatically. " +
                            "Make sure that all graphics resources are disposed manually to avoid memory leaks.",
                            ResourceName
                        ),
                        exception
                    );
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this resource has been disposed.
        /// </summary>
        public bool Disposed
        {
            get
            {
                return _disposed;
            }
        }
    }
}
