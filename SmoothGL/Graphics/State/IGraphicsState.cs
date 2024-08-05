namespace SmoothGL.Graphics.State;

/// <summary>
/// Encapsulates a number of rendering state variables.
/// </summary>
public interface IGraphicsState
{
    /// <summary>
    /// Communicates the state encapsulated in this state object to the driver.
    /// </summary>
    void Apply();
}