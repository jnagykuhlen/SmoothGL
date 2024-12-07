namespace SmoothGL.Content.Internal;

public class ContentCache(ContentFileHandler contentFileHandler) : IContentCache
{
    private readonly Dictionary<(Type, NormalizedPath), object> _cachedContentObjects = new();
    public T? GetCached<T>(string relativeFilePath) where T : class =>
        _cachedContentObjects.GetValueOrDefault((typeof(T), relativeFilePath)) as T;

    public T AddToCache<T>(string relativeFilePath, IContentReader<T> contentReader, IContentProvider contentProvider) where T : class
    {
        using var fileStream = contentFileHandler.OpenRead(relativeFilePath);
        var contentObject = contentReader.Read<T>(fileStream, contentProvider);
        
        _cachedContentObjects[(typeof(T), relativeFilePath)] = contentObject;

        return contentObject;
    }

    public void UpdateCached(IContentProvider contentProvider)
    {
    }

    public void Clear() => _cachedContentObjects.Clear();
}