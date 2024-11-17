using SmoothGL.Graphics.Geometry;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads a vertex array from a stream.
/// </summary>
public class VertexArrayReader : IContentReader<VertexArray>
{
    /// <summary>
    /// Reads a vertex array from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    public VertexArray Read(Stream stream, Type requestedType, ContentManager contentManager)
    {
        var meshData = contentManager.Load<MeshData>(stream);
        var vertexBuffer = contentManager.Add(meshData.ToVertexBuffer());
        var elementBuffer = contentManager.Add(meshData.ToElementBuffer());
        return new VertexArray(vertexBuffer, elementBuffer);
    }

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;
}