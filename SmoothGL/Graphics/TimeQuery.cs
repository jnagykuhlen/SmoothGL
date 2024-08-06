using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a query which can be used to measure the time required for a number of graphics operations.
/// </summary>
public class TimeQuery() : Query(QueryTarget.TimeElapsed)
{
    /// <summary>
    /// Gets the required time for all graphics operations between <see cref="Query.Begin" /> and <see cref="Query.End" />.
    /// This value is available when the query has finished.
    /// </summary>
    public TimeSpan Elapsed => TimeSpan.FromMicroseconds(Result / 1000.0);

    /// <summary>
    /// Gets the time since the start of the application.
    /// </summary>
    public static TimeSpan TotalElapsed
    {
        get
        {
            GL.GetInteger64(GetPName.Timestamp, out var timestamp);
            return TimeSpan.FromMicroseconds(timestamp / 1000.0);
        }
    }
}