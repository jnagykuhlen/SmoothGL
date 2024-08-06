using System.Globalization;
using OpenTK.Mathematics;
using SmoothGL.Graphics.Geometry;

namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which loads mesh data from a stream, in accordance with the wavefront OBJ specification.
/// </summary>
public class WavefrontObjReader : IContentReader<MeshData>
{
    /// <summary>
    /// Reads mesh data from a stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    public MeshData Read(Stream stream, Type requestedType, ContentManager contentManager)
    {
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var textureCoordinates = new List<Vector2>();
        var indices = new List<uint>();

        foreach (var line in ReadAllLines(stream))
        {
            if (line.StartsWith("vt "))
                textureCoordinates.Add(ParseTextureCoordinate(line[3..].Trim()));
            else if (line.StartsWith("vn "))
                normals.Add(ParseNormal(line[3..].Trim()));
            else if (line.StartsWith("v "))
                vertices.Add(ParseVertex(line[2..].Trim()));
            else if (line.StartsWith("f "))
                indices.AddRange(ParseFace(line[2..].Trim()));
        }

        return new MeshData(vertices.ToArray(), normals.ToArray(), textureCoordinates.ToArray(), indices.ToArray());
    }

    private static IEnumerable<string> ReadAllLines(Stream stream)
    {
        var streamReader = new StreamReader(stream);
        while (streamReader.ReadLine() is { } line)
            yield return line.Trim();
    }

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;

    /// <summary>
    /// Gets the name of this reader.
    /// </summary>
    public string ReaderName => "MeshDataReader";

    private float[] ParseVector(string text, int numberOfComponents)
    {
        var componentStrings = text.Split(' ');
        if (componentStrings.Length < numberOfComponents)
            throw new InvalidDataException("Wrong Wavefront OBJ file format.");

        try
        {
            return componentStrings
                .Where(componentString => !string.IsNullOrWhiteSpace(componentString))
                .Take(numberOfComponents)
                .Select(componentString => float.Parse(componentString.Trim(), CultureInfo.InvariantCulture))
                .ToArray();
        }
        catch (FormatException formatException)
        {
            throw new InvalidDataException($"Unable to parse vector string \"{text}\".", formatException);
        }
    }

    private Vector3 ParseVertex(string text)
    {
        var components = ParseVector(text, 3);
        return new Vector3(components[0], components[1], components[2]);
    }

    private Vector3 ParseNormal(string text)
    {
        var components = ParseVector(text, 3);
        return Vector3.Normalize(new Vector3(components[0], components[1], components[2]));
    }

    private Vector2 ParseTextureCoordinate(string text)
    {
        var components = ParseVector(text, 2);
        return new Vector2(components[0], components[1]);
    }

    private IEnumerable<uint> ParseFace(string text)
    {
        var indexStrings = text.Split(' ');
        if (indexStrings.Length < 3)
            throw new InvalidDataException("Wrong Wavefront OBJ file format.");

        return Triangulate(indexStrings.Select(indexString => ParseIndex(indexString.Trim())).ToArray());
    }

    private static IEnumerable<uint> Triangulate(uint[] indices)
    {
        var first = indices[0];
        var last = indices[1];

        foreach (var current in indices[2..])
        {
            yield return first;
            yield return last;
            yield return current;

            last = current;
        }
    }

    private uint ParseIndex(string text)
    {
        var indices = text.Split('/')
            .Where(indexString => !string.IsNullOrWhiteSpace(indexString))
            .Select(indexString => uint.Parse(indexString.Trim(), CultureInfo.InvariantCulture))
            .ToArray();

        if (indices.Any(index => index != indices[0]))
            throw new InvalidDataException("Wavefront OBJ loader does not support different indices for vertex components.");

        return indices[0] - 1;
    }
}