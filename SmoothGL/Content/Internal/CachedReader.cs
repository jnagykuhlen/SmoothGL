﻿namespace SmoothGL.Content.Internal;

/// <summary>
///     A proxy reader which wraps another reader. When an object has been already loaded,
///     this reader returns a cached version instead of requesting a new one from the internal reader.
/// </summary>
/// <typeparam name="T">Type of objects read.</typeparam>
public class CachedReader<T> : IContentReader<CachedResult>, ICachedReader
{
    private readonly Dictionary<string, T> _cache;
    private readonly IContentReader<T> _internalReader;

    /// <summary>
    ///     Creates a new cached reader which wraps around the specified reader.
    /// </summary>
    /// <param name="internalReader">The internal reader used to load objects which are not cached yet.</param>
    public CachedReader(IContentReader<T> internalReader)
    {
        if (internalReader == null)
            throw new ArgumentNullException("internalReader");

        _internalReader = internalReader;
        _cache = new Dictionary<string, T>();
    }

    /// <summary>
    ///     Clears the cache.
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <summary>
    ///     Reads an object from a stream using the internal reader if it is not cached.
    ///     Otherwise, the cached object is returned.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    public CachedResult Read(Stream stream, Type requestedType, ContentManager contentManager)
    {
        var fileStream = stream as FileStream;
        if (fileStream == null) return new CachedResult(_internalReader.Read(stream, requestedType, contentManager), true);

        var filename = fileStream.Name;

        T result;
        if (_cache.TryGetValue(filename, out result))
            return new CachedResult(result, false);

        result = _internalReader.Read(stream, requestedType, contentManager);
        _cache.Add(filename, result);

        return new CachedResult(result, true);
    }

    /// <summary>
    ///     Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => _internalReader.CanReadSubtypes;

    /// <summary>
    ///     Gets the name of this reader.
    /// </summary>
    public string ReaderName => "Cached" + _internalReader.ReaderName;
}