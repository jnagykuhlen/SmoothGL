using SmoothGL.Graphics;

namespace SmoothGL.Content;

/// <summary>
/// Specifies how the six individual faces of a cube texture are arranged in a two-dimensional image file.
/// A source image is interpreted as a uniform grid of quadratic tiles. Each cube face is then
/// selected by the coordinates of the corresponding tile in the grid.
/// </summary>
public class CubeTextureLayout
{
    /// <summary>
    /// Represents a layout in which the six cube face textures are arranged in a horizontal strip.
    /// </summary>
    public static readonly CubeTextureLayout HorizontalStrip = new(
        6, 1,
        new CubeTextureElement(1, 0),
        new CubeTextureElement(3, 0),
        new CubeTextureElement(5, 0),
        new CubeTextureElement(4, 0),
        new CubeTextureElement(0, 0),
        new CubeTextureElement(2, 0)
    );

    /// <summary>
    /// Represents a layout in which the six cube face textures are arranged in a horizontal cross,
    /// dividing the source image into a grid of 4x3 tiles. The top and bottom row of tiles is not taken
    /// into account except from the second tile each.
    /// </summary>
    public static readonly CubeTextureLayout HorizontalCross = new(
        4, 3,
        new CubeTextureElement(2, 1),
        new CubeTextureElement(0, 1),
        new CubeTextureElement(1, 0),
        new CubeTextureElement(1, 2),
        new CubeTextureElement(1, 1),
        new CubeTextureElement(3, 1)
    );

    private readonly CubeTextureElement[] _elements;

    private CubeTextureLayout(int gridWidth, int gridHeight, CubeTextureElement[] elements)
    {
        GridWidth = gridWidth;
        GridHeight = gridHeight;
        _elements = elements;
    }

    /// <summary>
    /// Creates a new cube texture layout, interpreting a source image as a grid of tiles of specified
    /// width and height. The cube face textures are selected as the specified tile in the grid.
    /// </summary>
    /// <param name="gridWidth">Number of tiles in the grid in horizontal direction.</param>
    /// <param name="gridHeight">Number of tiles in the grid in vertical direction.</param>
    /// <param name="elementPositiveX">Position of the tile in the grid corresponding to the positive X cube face.</param>
    /// <param name="elementNegativeX">Position of the tile in the grid corresponding to the negative X cube face.</param>
    /// <param name="elementPositiveY">Position of the tile in the grid corresponding to the positive Y cube face.</param>
    /// <param name="elementNegativeY">Position of the tile in the grid corresponding to the negative Y cube face.</param>
    /// <param name="elementPositiveZ">Position of the tile in the grid corresponding to the positive Z cube face.</param>
    /// <param name="elementNegativeZ">Position of the tile in the grid corresponding to the negative Z cube face.</param>
    public CubeTextureLayout(
        int gridWidth,
        int gridHeight,
        CubeTextureElement elementPositiveX,
        CubeTextureElement elementNegativeX,
        CubeTextureElement elementPositiveY,
        CubeTextureElement elementNegativeY,
        CubeTextureElement elementPositiveZ,
        CubeTextureElement elementNegativeZ)
    {
        if (gridWidth <= 0)
            throw new ArgumentOutOfRangeException("gridWidth", "Grid width must be positive.");

        if (gridHeight <= 0)
            throw new ArgumentOutOfRangeException("gridHeight", "Grid height must be positive.");

        GridWidth = gridWidth;
        GridHeight = gridHeight;

        _elements = new[]
        {
            elementPositiveX,
            elementNegativeX,
            elementPositiveY,
            elementNegativeY,
            elementPositiveZ,
            elementNegativeZ
        };

        foreach (var element in _elements)
        {
            if (element == null)
                throw new ArgumentNullException("element", "Cube texture layout elements must not contain null values.");

            if (element.GridX < 0 || element.GridX >= gridWidth || element.GridY < 0 || element.GridY >= gridHeight)
                throw new ArgumentException("Element location must be contained in the specified grid.", "elements");
        }
    }

    /// <summary>
    /// Gets the number of tiles in the grid in horizontal direction.
    /// </summary>
    public int GridWidth { get; }

    /// <summary>
    /// Gets the number of tiles in the grid in vertical direction.
    /// </summary>
    public int GridHeight { get; }

    /// <summary>
    /// Creates a new cube texture layout in which the six cube face textures are arranged in a horizontal strip,
    /// specifying the order in which each cube face appears in the strip of tiles.
    /// </summary>
    /// <param name="cubeFaceOrder">Defines the order in which the cube faces are stored in the source image file.</param>
    /// <returns>Cube texture layout.</returns>
    public static CubeTextureLayout FromCubeFaceOrder(TextureCubeFace[] cubeFaceOrder)
    {
        if (cubeFaceOrder == null)
            throw new ArgumentNullException("cubeFaceOrder");

        if (cubeFaceOrder.Length != 6)
            throw new ArgumentException("Cube face order array must contain exactly six values.", "cubeFaceOrder");

        var elements = new CubeTextureElement[6];
        for (var i = 0; i < 6; ++i)
            elements[(int)cubeFaceOrder[i]] = new CubeTextureElement(i, 0);

        if (elements.Any(e => e == null))
            throw new ArgumentException("Cube face order array must contain each face exactly once.", "cubeFaceOrder");

        return new CubeTextureLayout(6, 1, elements);
    }

    /// <summary>
    /// Gets the coordinates of the tile in the grid corresponding to the specified cube face.
    /// </summary>
    /// <param name="cubeFace">Cube face for which the tile coordinates in the grid are requested.</param>
    /// <returns>Position of the corresponding tile in the grid.</returns>
    public CubeTextureElement GetElement(TextureCubeFace cubeFace)
    {
        return _elements[(int)cubeFace];
    }
}