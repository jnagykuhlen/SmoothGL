namespace SmoothGL.Content.Internal;

/// <summary>
/// A proxy reader which wraps another reader. When an object has been already loaded,
/// this reader returns a cached version instead of requesting a new one from the internal reader.
/// </summary>
/// <typeparam name="T">Type of objects read.</typeparam>
public class CachedReader<T> : IContentReader<CachedResult>, ICachedReader where T : notnull
{
    private readonly Dictionary<string, T> _cache;
    private readonly IContentReader<T> _internalReader;

    /// <summary>
    /// Creates a new cached reader which wraps around the specified reader.
    /// </summary>
    /// <param name="internalReader">The internal reader used to load objects which are not cached yet.</param>
    public CachedReader(IContentReader<T> internalReader)
    {
        _internalReader = internalReader;
        _cache = new Dictionary<string, T>();
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Reads an object from a stream using the internal reader if it is not cached.
    /// Otherwise, the cached object is returned.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    public CachedResult Read(Stream stream, Type requestedType, ContentManager contentManager)
    {
        if (stream is not FileStream fileStream)
            return new CachedResult(_internalReader.Read(stream, requestedType, contentManager), true);

        var filePath = fileStream.Name;

        if (_cache.TryGetValue(filePath, out var cachedResult))
            return new CachedResult(cachedResult, false);

        var result = _internalReader.Read(stream, requestedType, contentManager);
        _cache.Add(filePath, result);

        return new CachedResult(result, true);
    }

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => _internalReader.CanReadSubtypes;
}

public record CachedResult(object Value, bool IsNew);