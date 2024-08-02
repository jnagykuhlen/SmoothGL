using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Describes how vertices from a vertex buffer are connected to each other when being drawn,
/// i.e., determines the primitive a fixed number of vertices form.
/// </summary>
public enum Primitive
{
    Triangles = PrimitiveType.Triangles,
    TrianglesAdjacency = PrimitiveType.TrianglesAdjacency,
    TriangleFan = PrimitiveType.TriangleFan,
    TriangleStrip = PrimitiveType.TriangleStrip,
    TriangleStripAdjacency = PrimitiveType.TriangleStripAdjacency,
    Lines = PrimitiveType.Lines,
    LinesAdjacency = PrimitiveType.LinesAdjacency,
    LineLoop = PrimitiveType.LineLoop,
    LineStrip = PrimitiveType.LineStrip,
    LineStripAdjacency = PrimitiveType.LineStripAdjacency,
    Quads = PrimitiveType.Quads,
    QuadStrip = PrimitiveType.QuadStrip,
    Patches = PrimitiveType.Patches,
    Points = PrimitiveType.Points
}