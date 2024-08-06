using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a query which can be used to measure the total number of fragments passing the
/// graphics pipeline.
/// </summary>
public class OcclusionQuery() : Query(QueryTarget.SamplesPassed)
{
    /// <summary>
    /// Gets the number of fragments which passed the graphics pipeline between <see cref="Query.Begin" /> and
    /// <see cref="Query.End" />.
    /// This value is available when the query has finished.
    /// </summary>
    public int FragmentCount => (int)Result;
}