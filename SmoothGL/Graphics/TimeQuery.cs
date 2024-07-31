using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Represents a query which can be used to measure the time required for a number of graphics operations.
/// </summary>
public class TimeQuery : Query
{
    public TimeQuery()
        : base(QueryTarget.TimeElapsed)
    {
    }

    /// <summary>
    /// Gets the required time for all graphics operations between <see cref="Query.Begin" /> and <see cref="Query.End" />,
    /// measured in nanoseconds.
    /// This value is available when the query has finished.
    /// </summary>
    public int ElapsedTime => Result;

    /// <summary>
    /// Gets the time since the start of the application, measured in nanoseconds.
    /// </summary>
    public static long Timestamp
    {
        get
        {
            long timestamp;
            GL.GetInteger64(GetPName.Timestamp, out timestamp);
            return timestamp;
        }
    }
}