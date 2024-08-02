using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Geometry;

/// <summary>
/// Represents a triangle mesh stored in client memory, defined by a number of vertices and corresponding indices.
/// Each vertex is defined by its position, normal and texture coordinate.
/// </summary>
public class MeshData
{
    /// <summary>
    /// Selector function which creates vertices that only contain position data, ignoring normals and texture coordinates.
    /// </summary>
    public static readonly Func<Vector3, Vector3, Vector2, VertexPosition> VertexPositionSelector =
        (position, _, _) => new VertexPosition(position);

    /// <summary>
    /// Selector function which creates vertices that contain position and texture coordinate data,
    /// ignoring normals.
    /// </summary>
    public static readonly Func<Vector3, Vector3, Vector2, VertexPositionTexture> VertexPositionTextureSelector =
        (position, _, textureCoordinate) => new VertexPositionTexture(position, textureCoordinate);

    /// <summary>
    /// Selector function which creates vertices that contain position, normal and texture coordinate data.
    /// </summary>
    public static readonly Func<Vector3, Vector3, Vector2, VertexPositionNormalTexture> VertexPositionNormalTextureSelector =
        (position, normal, textureCoordinate) => new VertexPositionNormalTexture(position, normal, textureCoordinate);

    private readonly Vector3[] _positions;
    private readonly Vector3[] _normals;
    private readonly Vector2[] _textureCoordinates;
    private readonly uint[]? _indices;

    /// <summary>
    /// Creates a new triangle mesh data set from a number of vertices and optional indices. When indices are specified,
    /// every triple of indices
    /// defines exactly one triangle. Otherwise, every three vertices represent one triangle.
    /// </summary>
    /// <param name="positions">The vertices' positions.</param>
    /// <param name="normals">The vertices' normal vectors, or null.</param>
    /// <param name="textureCoordinates">The vertices' texture coordinates, or null.</param>
    /// <param name="indices">The indices which reference the specified vertices, or null.</param>
    public MeshData(Vector3[] positions, Vector3[]? normals = null, Vector2[]? textureCoordinates = null, uint[]? indices = null)
    {
        _positions = positions;
        _normals = normals ?? [];
        _textureCoordinates = textureCoordinates ?? [];
        _indices = indices;
    }

    /// <summary>
    /// Gets the number of vertices of this mesh.
    /// </summary>
    public int NumberOfVertices => _positions.Length;

    /// <summary>
    /// Indicates whether this mesh has a list of indices. When indices exist, every triple of indices
    /// defines exactly one triangle. Otherwise, every three vertices represent one triangle.
    /// </summary>
    public bool HasIndices => _indices != null;

    /// <summary>
    /// Gets the number of indices of this mesh, or 0 if it has no indices.
    /// </summary>
    public int NumberOfIndices => _indices?.Length ?? 0;

    public static Vector3[] NormalsFromPositions(Vector3[] positions)
    {
        if (positions.Length % 3 != 0)
            throw new ArgumentException("The number of positions must be a multiple of three.", nameof(positions));

        var normals = new Vector3[positions.Length];
        for (var i = 0; i < positions.Length; i += 3)
        {
            var triangleNormal = Vector3.Cross(positions[i + 1] - positions[i], positions[i + 2] - positions[i]).Normalized();
            for (var j = 0; j < 3; ++j)
                normals[i + j] = triangleNormal;
        }

        return normals;
    }

    /// <summary>
    /// Gets an array of vertices of the specified vertex type, transformed by a selector function.
    /// </summary>
    /// <typeparam name="T">Vertex type.</typeparam>
    /// <param name="selector">
    /// Selector function which creates a single vertex of type T from a position, normal and texture
    /// coordinate.
    /// </param>
    /// <returns>Array of vertices.</returns>
    public T[] GetVertices<T>(Func<Vector3, Vector3, Vector2, T> selector)
    {
        return _positions
            .Select((position, index) => selector(
                position,
                ElementOrDefault(_normals, index),
                ElementOrDefault(_textureCoordinates, index))
            )
            .ToArray();
    }

    /// <summary>
    /// Gets the array of vertices, holding position, normal and texture coordinate information.
    /// </summary>
    /// <returns>Array of vertices.</returns>
    public VertexPositionNormalTexture[] GetVertices() => GetVertices(VertexPositionNormalTextureSelector);

    private static T ElementOrDefault<T>(T[] array, int index) where T : struct =>
        index < array.Length ? array[index] : default;

    /// <summary>
    /// Gets the array of indices as unsigned bytes.
    /// </summary>
    /// <returns>Array of indices.</returns>
    public byte[] GetIndicesUnsignedByte() => _indices?.Select(index => (byte)index).ToArray() ?? [];

    /// <summary>
    /// Gets the array of indices as unsigned shorts.
    /// </summary>
    /// <returns>Array of indices.</returns>
    public ushort[] GetIndicesUnsignedShort() => _indices?.Select(index => (ushort)index).ToArray() ?? [];

    /// <summary>
    /// Gets the array of indices as unsigned integers.
    /// </summary>
    /// <returns>Array of indices.</returns>
    public uint[] GetIndicesUnsignedInt() => _indices == null ? [] : (uint[])_indices.Clone();

    /// <summary>
    /// Creates a vertex buffer stored on the GPU from the meshes' vertices, transformed by a selector function.
    /// </summary>
    /// <typeparam name="T">Vertex type.</typeparam>
    /// <param name="selector">
    /// Selector function which creates a single vertex of type T from a position, normal and texture
    /// coordinate.
    /// </param>
    /// <param name="vertexDeclaration">Vertex declaration associated with vertex type T.</param>
    /// <returns>Vertex buffer.</returns>
    public VertexBuffer ToVertexBuffer<T>(Func<Vector3, Vector3, Vector2, T> selector, VertexDeclaration vertexDeclaration)
        where T : struct
    {
        var vertexBuffer = new VertexBuffer(NumberOfVertices, vertexDeclaration, BufferUsage.Static);
        vertexBuffer.SetData(GetVertices(selector));
        return vertexBuffer;
    }

    /// <summary>
    /// Creates a vertex buffer stored on the GPU from the mesh's vertices, defining a position, normal and texture
    /// coordinate each.
    /// </summary>
    /// <returns>Vertex buffer.</returns>
    public VertexBuffer ToVertexBuffer() =>
        ToVertexBuffer(VertexPositionNormalTextureSelector, VertexPositionNormalTexture.VertexDeclaration);

    /// <summary>
    /// Creates an element buffer from the mesh's indices. The minimum required number of bits per index is determined
    /// automatically.
    /// </summary>
    /// <returns>Element buffer.</returns>
    public ElementBuffer ToElementBuffer()
    {
        if (_indices == null)
            throw new InvalidOperationException("Cannot create element buffer from mesh data that has no indices.");

        ElementBuffer elementBuffer;
        switch (NumberOfVertices)
        {
            case <= byte.MaxValue:
                elementBuffer = new ElementBuffer(NumberOfIndices, ElementType.UnsignedByte, BufferUsage.Static);
                elementBuffer.SetData(GetIndicesUnsignedByte());
                break;
            case <= ushort.MaxValue:
                elementBuffer = new ElementBuffer(NumberOfIndices, ElementType.UnsignedShort, BufferUsage.Static);
                elementBuffer.SetData(GetIndicesUnsignedShort());
                break;
            default:
                elementBuffer = new ElementBuffer(NumberOfIndices, ElementType.UnsignedInt, BufferUsage.Static);
                elementBuffer.SetData(GetIndicesUnsignedInt());
                break;
        }

        return elementBuffer;
    }

    /// <summary>
    /// Creates new mesh data from this mesh with identical vertices, but flipped triangle orientation.
    /// </summary>
    /// <returns>Mesh data with flipped triangle orientation.</returns>
    public MeshData GetFlippedTriangleOrientation()
    {
        if (_indices == null)
            return new MeshData(FlipTriangleOrientation(_positions), _normals, _textureCoordinates);

        return new MeshData(_positions, _normals, _textureCoordinates, FlipTriangleOrientation(_indices));
    }

    private static T[] FlipTriangleOrientation<T>(T[] elements)
    {
        var flippedElements = (T[])elements.Clone();

        var i = 0;
        while (i + 1 < flippedElements.Length)
        {
            Swap(ref flippedElements[i], ref flippedElements[i + 1]);
            i += 3;
        }

        return flippedElements;
    }

    private static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }
}