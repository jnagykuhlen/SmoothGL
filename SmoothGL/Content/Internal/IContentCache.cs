namespace SmoothGL.Content.Internal;

public interface IContentCache
{
    T? GetCached<T>(string relativeFilePath) where T : class;
    T AddToCache<T>(IContentReader<T> contentReader, string relativeFilePath, IContentProvider contentProvider) where T : class;
    void UpdateCached(IContentProvider contentProvider);
    void Clear();
}