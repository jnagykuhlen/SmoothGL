using OpenTK.Mathematics;

namespace SmoothGL.Graphics;

/// <summary>
///     Represents a triangle mesh stored in client memory, defined by a number of vertices and corresponding indices.
///     Each vertex is defined by its position, normal and texture coordinate.
/// </summary>
public class MeshData
{
    /// <summary>
    ///     Selector function which creates vertices that only contain position data, ignoring normals and texture coordinates.
    /// </summary>
    public static readonly Func<Vector3, Vector3, Vector2, VertexPosition> VertexPositionSelector =
        (p, n, t) => new VertexPosition(p);

    /// <summary>
    ///     Selector function which creates vertices that contain position and texture coordinate data,
    ///     ignoring normals.
    /// </summary>
    public static readonly Func<Vector3, Vector3, Vector2, VertexPositionTexture> VertexPositionTextureSelector =
        (p, n, t) => new VertexPositionTexture(p, t);

    /// <summary>
    ///     Selector function which creates vertices that contain position, normal and texture coordinate data.
    /// </summary>
    public static readonly Func<Vector3, Vector3, Vector2, VertexPositionNormalTexture> VertexPositionNormalTextureSelector =
        (p, n, t) => new VertexPositionNormalTexture(p, n, t);

    private readonly uint[] _indices;
    private readonly Vector3[] _normals;

    private readonly Vector3[] _positions;
    private readonly Vector2[] _textureCoordinates;

    /// <summary>
    ///     Creates a new triangle mesh data set from a number of vertices, where every triple of vertices defines exactly one
    ///     triangle.
    ///     Vertex normals are derived per triangle.
    /// </summary>
    /// <param name="positions">The vertices' positions.</param>
    /// <param name="textureCoordinates">The vertices' texture coordinates, or null.</param>
    public MeshData(Vector3[] positions, Vector2[] textureCoordinates)
        : this(positions, NormalsFromPositions(positions), textureCoordinates, null)
    {
    }

    /// <summary>
    ///     Creates a new triangle mesh data set from a number of vertices and optional indices. When indices are specified,
    ///     every triple of indices
    ///     defines exactly one triangle. Otherwise, every three vertices represent one triangle.
    /// </summary>
    /// <param name="positions">The vertices' positions.</param>
    /// <param name="normals">The vertices' normal vectors, or null.</param>
    /// <param name="textureCoordinates">The vertices' texture coordinates, or null.</param>
    /// <param name="indices">The indices which reference the specified vertices, or null.</param>
    public MeshData(Vector3[] positions, Vector3[] normals, Vector2[] textureCoordinates, uint[] indices)
    {
        if (positions == null)
            throw new ArgumentNullException("positions");

        _positions = positions;
        _normals = normals ?? new Vector3[0];
        _textureCoordinates = textureCoordinates ?? new Vector2[0];
        _indices = indices;
    }

    /// <summary>
    ///     Gets the number of vertices of this mesh.
    /// </summary>
    public int NumberOfVertices => _positions.Length;

    /// <summary>
    ///     Indicates whether this mesh has a list of indices. When indices exist, every triple of indices
    ///     defines exactly one triangle. Otherwise, every three vertices represent one triangle.
    /// </summary>
    public bool HasIndices => _indices != null;

    /// <summary>
    ///     Gets the number of indices of this mesh, or 0 if it has no indices.
    /// </summary>
    public int NumberOfIndices
    {
        get
        {
            if (_indices == null)
                return 0;

            return _indices.Length;
        }
    }

    private static Vector3[] NormalsFromPositions(Vector3[] positions)
    {
        if (positions == null)
            throw new ArgumentNullException("positions");

        if (positions.Length % 3 != 0)
            throw new ArgumentException("The number of positions must be a multiple of three.", "positions");

        var normals = new Vector3[positions.Length];
        for (var i = 0; i < positions.Length; i += 3)
        {
            var triangleNormal = Vector3.Cross(positions[i + 1] - positions[i], positions[i + 2] - positions[i]).Normalized();
            for (var j = 0; j < 3; ++j)
                normals[i + j] = triangleNormal;
        }

        return normals;
    }

    private static T ElementOrDefault<T>(T[] array, int index)
    {
        if (index < array.Length)
            return array[index];
        return default;
    }

    /// <summary>
    ///     Gets an array of vertices of the specified vertex type, transformed by a selector function.
    /// </summary>
    /// <typeparam name="T">Vertex type.</typeparam>
    /// <param name="selector">
    ///     Selector function which creates a single vertex of type T from a position, normal and texture
    ///     coordinate.
    /// </param>
    /// <returns>Array of vertices.</returns>
    public T[] GetVertices<T>(Func<Vector3, Vector3, Vector2, T> selector)
    {
        var result = new T[NumberOfVertices];
        for (var i = 0; i < NumberOfVertices; ++i)
            result[i] = selector(
                ElementOrDefault(_positions, i),
                ElementOrDefault(_normals, i),
                ElementOrDefault(_textureCoordinates, i)
            );
        return result;
    }

    /// <summary>
    ///     Gets the array of vertices, holding position, normal and texture coordinate information.
    /// </summary>
    /// <returns>Array of vertices.</returns>
    public VertexPositionNormalTexture[] GetVertices()
    {
        return GetVertices(VertexPositionNormalTextureSelector);
    }

    /// <summary>
    ///     Gets the array of indices as unsigned bytes.
    /// </summary>
    /// <returns>Array of indices.</returns>
    public byte[] GetIndicesUnsignedByte()
    {
        if (_indices == null)
            return null;

        return _indices.Select(i => (byte)i).ToArray();
    }

    /// <summary>
    ///     Gets the array of indices as unsigned shorts.
    /// </summary>
    /// <returns>Array of indices.</returns>
    public ushort[] GetIndicesUnsignedShort()
    {
        if (_indices == null)
            return null;

        return _indices.Select(i => (ushort)i).ToArray();
    }

    /// <summary>
    ///     Gets the array of indices as unsigned integers.
    /// </summary>
    /// <returns>Array of indices.</returns>
    public uint[] GetIndicesUnsignedInt()
    {
        if (_indices == null)
            return null;

        return (uint[])_indices.Clone();
    }

    /// <summary>
    ///     Creates a vertex buffer stored on the GPU from the meshes' vertices, transformed by a selector function.
    /// </summary>
    /// <typeparam name="T">Vertex type.</typeparam>
    /// <param name="selector">
    ///     Selector function which creates a single vertex of type T from a position, normal and texture
    ///     coordinate.
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
    ///     Creates a vertex buffer stored on the GPU from the mesh's vertices, defining a position, normal and texture
    ///     coordinate each.
    /// </summary>
    /// <returns>Vertex buffer.</returns>
    public VertexBuffer ToVertexBuffer()
    {
        return ToVertexBuffer(VertexPositionNormalTextureSelector, VertexPositionNormalTexture.VertexDeclaration);
    }

    /// <summary>
    ///     Creates an element buffer from the mesh's indices. The minimum required number of bits per index is determined
    ///     automatically.
    /// </summary>
    /// <returns>Element buffer.</returns>
    public ElementBuffer ToElementBuffer()
    {
        if (_indices == null)
            throw new InvalidOperationException("Cannot create element buffer from mesh data that has no indices.");

        ElementBuffer elementBuffer;
        if (NumberOfVertices <= byte.MaxValue)
        {
            elementBuffer = new ElementBuffer(NumberOfIndices, ElementType.UnsignedByte, BufferUsage.Static);
            elementBuffer.SetData(GetIndicesUnsignedByte());
        }
        else if (NumberOfVertices <= ushort.MaxValue)
        {
            elementBuffer = new ElementBuffer(NumberOfIndices, ElementType.UnsignedShort, BufferUsage.Static);
            elementBuffer.SetData(GetIndicesUnsignedShort());
        }
        else
        {
            elementBuffer = new ElementBuffer(NumberOfIndices, ElementType.UnsignedInt, BufferUsage.Static);
            elementBuffer.SetData(GetIndicesUnsignedInt());
        }

        return elementBuffer;
    }

    /// <summary>
    ///     Creates new mesh data from this mesh with identical vertices, but flipped triangle orientation.
    /// </summary>
    /// <returns>Mesh data with flipped triangle orientation.</returns>
    public MeshData GetFlippedTriangleOrientation()
    {
        if (_indices == null)
            return new MeshData(FlipTriangleOrientation(_positions), _normals, _textureCoordinates, _indices);

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
        var temp = a;
        a = b;
        b = temp;
    }
}