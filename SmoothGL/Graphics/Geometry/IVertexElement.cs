namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Defines an atomic element vertex data can be composed of.
/// </summary>
public interface IVertexElement
{
    /// <summary>
    /// Gets the number of bytes required to represent this vertex element in memory.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Communicates this vertex element definition to the GPU. This method is not required to be called by client code.
    /// </summary>
    /// <param name="strideSize">The length of a single vertex representation in memory, in bytes.</param>
    /// <param name="offset">The offset at which this element is placed, in bytes.</param>
    /// <param name="divisor">The divisor.</param>
    void ApplyDefinition(int strideSize, int offset, int divisor);
}