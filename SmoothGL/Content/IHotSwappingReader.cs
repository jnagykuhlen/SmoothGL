namespace SmoothGL.Content;

public interface IHotSwappingReader
{
    void ReadInto(object existingObject, Stream stream, IContentProvider contentProvider);
}