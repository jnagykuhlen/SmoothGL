namespace SmoothGL.Content;

public interface IHotSwappingContentReader<in T> : IContentReader<T> where T : notnull
{
    void ReadInto(T existingObject, Stream stream, IContentProvider contentProvider);
}