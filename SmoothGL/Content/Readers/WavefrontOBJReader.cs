using System.Globalization;
using OpenTK.Mathematics;
using SmoothGL.Graphics;

namespace SmoothGL.Content;

/// <summary>
///     Reader class which loads mesh data from a stream, in accordance with the wavefront OBJ specification.
/// </summary>
public class WavefrontOBJReader : IContentReader<MeshData>
{
    /// <summary>
    ///     Reads mesh data from a stream.
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

        var reader = new StreamReader(stream);

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (line.StartsWith("vt"))
                textureCoordinates.Add(ParseTextureCoordinate(line.Substring(2, line.Length - 2).Trim()));
            else if (line.StartsWith("vn"))
                normals.Add(ParseNormal(line.Substring(2, line.Length - 2).Trim()));
            else if (line.StartsWith("v"))
                vertices.Add(ParseVertex(line.Substring(1, line.Length - 1).Trim()));
            else if (line.StartsWith("f")) indices.AddRange(ParseFace(line.Substring(1, line.Length - 1).Trim()));
        }

        return new MeshData(vertices.ToArray(), normals.ToArray(), textureCoordinates.ToArray(), indices.ToArray());
    }

    /// <summary>
    ///     Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;

    /// <summary>
    ///     Gets the name of this reader.
    /// </summary>
    public string ReaderName => "MeshDataReader";

    private float[] ParseVector(string text, int numberOfComponents)
    {
        var componentStrings = text.Split(' ');
        if (componentStrings.Length < numberOfComponents)
            throw new InvalidDataException("Wrong Wavefront OBJ file format.");

        try
        {
            return componentStrings.Where(s => !string.IsNullOrWhiteSpace(s)).Take(numberOfComponents)
                .Select(s => float.Parse(s.Trim(), CultureInfo.InvariantCulture))
                .ToArray();
        }
        catch (FormatException formatException)
        {
            throw new InvalidDataException(string.Format("Unable to parse vector string \"{0}\".", text), formatException);
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

        return Triangulate(indexStrings.Select(s => ParseIndex(s.Trim())));
    }

    private IEnumerable<uint> Triangulate(IEnumerable<uint> indices)
    {
        var first = indices.ElementAt(0);
        var last = indices.ElementAt(1);

        foreach (var current in indices.Skip(2))
        {
            yield return first;
            yield return last;
            yield return current;

            last = current;
        }
    }

    private uint ParseIndex(string text)
    {
        var indices = text.Split('/').Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => uint.Parse(s.Trim(), CultureInfo.InvariantCulture))
            .ToArray();

        if (indices.Any(i => i != indices[0]))
            throw new InvalidDataException("Wavefront OBJ loader does not support different indices for vertex components.");

        return indices[0] - 1;
    }
}