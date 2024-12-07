namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which reads a string from a stream.
/// </summary>
public class StringReader : ContentReader<string>
{
    protected override string Read(Stream stream, IContentProvider contentProvider)
    {
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}