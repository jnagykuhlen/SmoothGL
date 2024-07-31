namespace SmoothGL.Content;

/// <summary>
/// Specifies the position of a tile in a source image corresponding to a cube face.
/// </summary>
public class CubeTextureElement
{
    /// <summary>
    /// Creates a new cube texture element, specifying the position of the corresponding tile in a source image.
    /// </summary>
    /// <param name="gridX">The horizontal index of the tile in the grid.</param>
    /// <param name="gridY">The vertical index of the tile in the grid.</param>
    public CubeTextureElement(int gridX, int gridY)
    {
        GridX = gridX;
        GridY = gridY;
    }

    /// <summary>
    /// Gets the horizontal index of the tile in the grid.
    /// </summary>
    public int GridX { get; }

    /// <summary>
    /// Gets the vertical index of the tile in the grid.
    /// </summary>
    public int GridY { get; }
}