using SmoothGL.Graphics.Geometry;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads a vertex array from a stream.
/// </summary>
public class VertexArrayReader : ContentReader<VertexArray>
{
    protected override VertexArray Read(Stream stream, IContentProvider contentProvider)
    {
        var meshData = contentProvider.Load<MeshData>(stream);
        var vertexBuffer = contentProvider.Add(meshData.ToVertexBuffer());
        var elementBuffer = contentProvider.Add(meshData.ToElementBuffer());
        return new VertexArray(vertexBuffer, elementBuffer);
    }
}