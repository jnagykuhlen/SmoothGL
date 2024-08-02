namespace SmoothGL.Graphics.Geometry.Builder;

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