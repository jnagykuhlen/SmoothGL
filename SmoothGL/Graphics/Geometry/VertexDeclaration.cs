namespace SmoothGL.Graphics;

/// <summary>
/// Defines the layout of vertex elements forming a single vertex.
/// </summary>
public class VertexDeclaration
{
    private readonly IVertexElement[] _elements;

    /// <summary>
    /// Creates a new vertex declaration, defining a vertex' layout by the specified vertex elements.
    /// The stride size (the number of bytes allocated in a buffer for a single vertex) is determined as the
    /// sum of bytes required for the vertex elements. Therefore, the vertices are expected to be tightly packed.
    /// </summary>
    /// <param name="elements">Vertex elements which define the layout of a single vertex.</param>
    public VertexDeclaration(params IVertexElement[] elements)
        : this(elements.Sum(e => e.Size), elements)
    {
    }

    /// <summary>
    /// Creates a new vertex declaration, defining a vertex' layout by the specified vertex elements.
    /// </summary>
    /// <param name="strideSize">
    /// Number of bytes allocated in a buffer for a single vertex. Should be greater or equal to the
    /// sum of bytes required for the vertex elements.
    /// </param>
    /// <param name="elements">Vertex elements which define the layout of a single vertex.</param>
    public VertexDeclaration(int strideSize, params IVertexElement[] elements)
    {
        _elements = elements;
        StrideSize = strideSize;
    }

    /// <summary>
    /// Gets the stride size, which is the number of bytes allocated in a buffer for a single vertex.
    /// </summary>
    public int StrideSize { get; }

    /// <summary>
    /// Communicates this vertex declaration to the driver, affecting the currently bound vertex buffer and vertex array.
    /// This method is not required to be called from client code.
    /// </summary>
    /// <param name="isInstanceData">Indicates whether this vertex declaration describes data used for instanced rendering.</param>
    public void ApplyDefinition(bool isInstanceData)
    {
        var offset = 0;
        for (var i = 0; i < _elements.Length; ++i)
        {
            _elements[i].ApplyDefinition(StrideSize, offset, isInstanceData ? 1 : 0);
            offset += _elements[i].Size;
        }
    }
}