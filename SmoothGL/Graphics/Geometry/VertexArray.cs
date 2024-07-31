using OpenTK.Graphics.OpenGL;
using SmoothGL.Graphics.Internal;

namespace SmoothGL.Graphics;

/// <summary>
///     Stores references to vertex buffers and an optional element buffer to encapsulate a common configuration.
///     This configuration can then be drawn with a single method call. Disposing a vertex array does not dispose the
///     referenced buffers.
/// </summary>
public class VertexArray : GraphicsResource
{
    private static readonly VertexBuffer[] NoVertexBuffers = new VertexBuffer[0];
    private static int _currentVertexArrayId;
    private readonly int _defaultNumberOfElements;
    private readonly IDrawStrategy _drawStrategy;

    private int _vertexArrayId;

    private VertexArray()
    {
        GL.GenVertexArrays(1, out _vertexArrayId);
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffers.
    /// </summary>
    /// <param name="vertexBuffers">Referenced vertex buffers.</param>
    public VertexArray(VertexBuffer[] vertexBuffers)
        : this(vertexBuffers, NoVertexBuffers)
    {
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffer.
    /// </summary>
    /// <param name="vertexBuffer">Referenced vertex buffer.</param>
    public VertexArray(VertexBuffer vertexBuffer)
        : this(new[] { vertexBuffer }, NoVertexBuffers)
    {
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffers and element buffer.
    /// </summary>
    /// <param name="vertexBuffers">Referenced vertex buffers.</param>
    /// <param name="elementBuffer">Referenced element buffer.</param>
    public VertexArray(VertexBuffer[] vertexBuffers, ElementBuffer elementBuffer)
        : this(vertexBuffers, NoVertexBuffers, elementBuffer)
    {
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex and element buffer.
    /// </summary>
    /// <param name="vertexBuffer">Referenced vertex buffer.</param>
    /// <param name="elementBuffer">Referenced element buffer.</param>
    public VertexArray(VertexBuffer vertexBuffer, ElementBuffer elementBuffer)
        : this(new[] { vertexBuffer }, NoVertexBuffers, elementBuffer)
    {
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffer,
    ///     element buffer and an additional buffer holding per-instance data.
    /// </summary>
    /// <param name="vertexBuffer">Referenced vertex buffer.</param>
    /// <param name="instanceBuffer">Referenced buffer holding per-instance data.</param>
    /// <param name="elementBuffer">Referenced element buffer.</param>
    public VertexArray(VertexBuffer vertexBuffer, VertexBuffer instanceBuffer, ElementBuffer elementBuffer)
        : this(new[] { vertexBuffer }, new[] { instanceBuffer }, elementBuffer)
    {
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffers,
    ///     element buffer and an additional buffer holding per-instance data.
    /// </summary>
    /// <param name="vertexBuffers">Referenced vertex buffers.</param>
    /// <param name="instanceBuffer">Referenced buffer holding per-instance data.</param>
    /// <param name="elementBuffer">Referenced element buffer.</param>
    public VertexArray(VertexBuffer[] vertexBuffers, VertexBuffer instanceBuffer, ElementBuffer elementBuffer)
        : this(vertexBuffers, new[] { instanceBuffer }, elementBuffer)
    {
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffer and an additional buffer holding per-instance
    ///     data.
    /// </summary>
    /// <param name="vertexBuffer">Referenced vertex buffer.</param>
    /// <param name="instanceBuffer">Referenced buffer holding per-instance data.</param>
    public VertexArray(VertexBuffer vertexBuffer, VertexBuffer instanceBuffer)
        : this(new[] { vertexBuffer }, new[] { instanceBuffer })
    {
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffers and an additional buffer holding per-instance
    ///     data.
    /// </summary>
    /// <param name="vertexBuffers">Referenced vertex buffers.</param>
    /// <param name="instanceBuffer">Referenced buffer holding per-instance data.</param>
    public VertexArray(VertexBuffer[] vertexBuffers, VertexBuffer instanceBuffer)
        : this(vertexBuffers, new[] { instanceBuffer })
    {
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffers and additional buffers holding per-instance
    ///     data.
    /// </summary>
    /// <param name="vertexBuffers">Referenced vertex buffers.</param>
    /// <param name="instanceBuffers">Referenced buffers holding per-instance data.</param>
    public VertexArray(VertexBuffer[] vertexBuffers, VertexBuffer[] instanceBuffers)
        : this()
    {
        Bind();
        foreach (var vertexBuffer in vertexBuffers)
            AttachVertexBuffer(vertexBuffer, false);

        foreach (var instanceBuffer in instanceBuffers)
            AttachVertexBuffer(instanceBuffer, true);

        _defaultNumberOfElements = vertexBuffers.Min(v => v.NumberOfVertices);
        _drawStrategy = new ArrayDrawStrategy();
        Unbind();
    }

    /// <summary>
    ///     Creates a new vertex array referencing the specified vertex buffers,
    ///     element buffer and additional buffers holding per-instance data.
    /// </summary>
    /// <param name="vertexBuffers">Referenced vertex buffers.</param>
    /// <param name="instanceBuffers">Referenced buffers holding per-instance data.</param>
    /// <param name="elementBuffer">Referenced element buffer.</param>
    public VertexArray(VertexBuffer[] vertexBuffers, VertexBuffer[] instanceBuffers, ElementBuffer elementBuffer)
        : this()
    {
        Bind();
        elementBuffer.Bind();

        foreach (var vertexBuffer in vertexBuffers)
            AttachVertexBuffer(vertexBuffer, false);

        foreach (var instanceBuffer in instanceBuffers)
            AttachVertexBuffer(instanceBuffer, true);

        _defaultNumberOfElements = elementBuffer.NumberOfElements;
        _drawStrategy = new ElementDrawStrategy(elementBuffer.ElementType, elementBuffer.ElementSize);
        Unbind();
    }

    protected int Id => _vertexArrayId;

    protected override string ResourceName => "VertexArray";

    /// <summary>
    ///     Invalidates the binding cache used to speed up vertex array binding operations.
    ///     This method is not required to be called by client code.
    /// </summary>
    public static void InvalidateBindingCache()
    {
        _currentVertexArrayId = 0;
    }

    private void AttachVertexBuffer(VertexBuffer vertexBuffer, bool isInstanceData)
    {
        vertexBuffer.Bind();
        vertexBuffer.VertexDeclaration.ApplyDefinition(isInstanceData);
    }

    private void Bind(int vertexArrayId)
    {
        CheckDisposed();
        if (_currentVertexArrayId != vertexArrayId)
        {
            Buffer.InvalidateBindingCache();
            GL.BindVertexArray(vertexArrayId);
            _currentVertexArrayId = vertexArrayId;
        }
    }

    /// <summary>
    ///     Binds this vertex array to the graphics device. This method is not required to be called by client code.
    /// </summary>
    public void Bind()
    {
        Bind(_vertexArrayId);
    }

    /// <summary>
    ///     Unbinds this vertex array. This method is not required to be called by client code.
    /// </summary>
    public void Unbind()
    {
        Bind(0);
    }

    /// <summary>
    ///     Draws the geometry defined by the data in vertex buffers and element buffer,
    ///     interpreted as a number of specified primitives.
    /// </summary>
    /// <param name="primitiveType">Type of primitives the vertices form.</param>
    public void Draw(Primitive primitiveType)
    {
        Draw(primitiveType, 0, _defaultNumberOfElements);
    }

    /// <summary>
    ///     Draws the geometry defined by the data in vertex buffers and element buffer,
    ///     interpreted as a number of specified primitives.
    /// </summary>
    /// <param name="primitiveType">Type of primitives the vertices form.</param>
    /// <param name="startElement">Index of the first element taken into account.</param>
    /// <param name="numberOfElements">Number of elements from the buffers required to be taken into account.</param>
    public void Draw(Primitive primitiveType, int startElement, int numberOfElements)
    {
        Bind();
        _drawStrategy.Draw(primitiveType, startElement, numberOfElements);
        Unbind();
    }

    /// <summary>
    ///     Draws the geometry defined by the data in vertex buffers and element buffer multiple times,
    ///     interpreted as a number of specified primitives. When an instance buffer is referenced, its
    ///     unique per-instance data is available in the shader. Otherwise, the different instances
    ///     can be distinguished in the shader by instance ID.
    /// </summary>
    /// <param name="primitiveType">Type of primitives the vertices form.</param>
    /// <param name="numberOfInstances">Number of instances of this geometry that need to be drawn.</param>
    public void DrawMultiple(Primitive primitiveType, int numberOfInstances)
    {
        DrawMultiple(primitiveType, 0, _defaultNumberOfElements, numberOfInstances);
    }

    /// <summary>
    ///     Draws the geometry defined by the data in vertex buffers and element buffer multiple times,
    ///     interpreted as a number of specified primitives. When an instance buffer is referenced, its
    ///     unique per-instance data is available in the shader. Otherwise, the different instances
    ///     can be distinguished in the shader by instance ID.
    /// </summary>
    /// <param name="primitiveType">Type of primitives the vertices form.</param>
    /// ///
    /// <param name="startElement">Index of the first element taken into account.</param>
    /// <param name="numberOfElements">Number of elements from the buffers required to be taken into account.</param>
    /// <param name="numberOfInstances">Number of instances of this geometry that need to be drawn.</param>
    public void DrawMultiple(Primitive primitiveType, int startElement, int numberOfElements, int numberOfInstances)
    {
        Bind();
        _drawStrategy.DrawMultiple(primitiveType, startElement, numberOfElements, numberOfInstances);
        Unbind();
    }

    protected override void FreeResources()
    {
        GL.DeleteVertexArrays(1, ref _vertexArrayId);
    }
}