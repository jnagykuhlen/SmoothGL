using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a builder which can be used to construct mesh geometry in memory.
    /// </summary>
    public interface IGeometryBuilder
    {
        /// <summary>
        /// Builds a mesh stored in memory.
        /// </summary>
        /// <returns>Constructed mesh.</returns>
        MeshData Build();
    }
}
