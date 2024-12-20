using SmoothGL.Content.Internal;

namespace SmoothGL.Content;

/// <summary>
/// Handles loading of content from files or streams and takes care of disposing loaded content automatically.
/// <param name="rootPath">Root directory the paths of content files are relative to.</param>
/// <param name="enableHotSwapping">Indicates whether content files should be monitored for changes which are
/// automatically hot swapped into the loaded objects when <see cref="UpdateContent"/> is called.</param>
/// </summary>
public class ContentManager(string rootPath, bool enableHotSwapping = false) : IContentProvider, IDisposable
{
    private readonly ContentReaders _contentReaders = new();
    private readonly List<IDisposable> _disposables = new();
    private readonly IContentCache _contentCache = CreateContentCache(new ContentDirectory(rootPath), enableHotSwapping);

    private bool _disposed;

    private static IContentCache CreateContentCache(ContentDirectory contentDirectory, bool enableHotSwapping) =>
        enableHotSwapping
            ? new HotSwappingContentCache(contentDirectory)
            : new ContentCache(contentDirectory);

    /// <summary>
    /// Registers a content reader which handles loading of content of the specified type.
    /// When a reader is already registered for this type, it will be replaced. The content manager can be
    /// allowed to cache created content objects of this type to reduce loading times when the same file is
    /// requested multiple times. However, caching can cause problems when content objects are not immutable.
    /// </summary>
    /// <typeparam name="T">Content type which can be loaded by the reader.</typeparam>
    /// <param name="contentReader">Content reader of the specified type.</param>
    /// <returns>This content manager instance.</returns>
    public ContentManager SetContentReader<T>(IContentReader<T> contentReader) where T : notnull
    {
        _contentReaders.SetContentReader(contentReader);
        return this;
    }

    /// <summary>
    /// Loads content from a file.
    /// </summary>
    /// <typeparam name="T">The requested content type to load.</typeparam>
    /// <param name="relativeFilePath">Relative path to the file storing content data.</param>
    /// <returns>Content object.</returns>
    public T Load<T>(string relativeFilePath) where T : class
    {
        CheckDisposed();

        var existingContentObject = _contentCache.GetCached<T>(relativeFilePath);
        if (existingContentObject != null)
            return existingContentObject;

        var newContentObject = _contentCache.AddToCache(
            relativeFilePath,
            _contentReaders.GetContentReader<T>(),
            this
        );

        if (newContentObject is IDisposable disposable)
            _disposables.Add(disposable);

        return newContentObject;
    }

    /// <summary>
    /// Loads content from a stream.
    /// </summary>
    /// <typeparam name="T">The requested content type to load.</typeparam>
    /// <param name="stream">Stream from which content data is read.</param>
    /// <returns>Content object.</returns>
    public T Load<T>(Stream stream) where T : class
    {
        CheckDisposed();

        var newContentObject = _contentReaders.GetContentReader<T>().Read<T>(stream, this);
        if (newContentObject is IDisposable disposable)
            _disposables.Add(disposable);

        return newContentObject;
    }

    /// <summary>
    /// Adds a disposable content object so that its lifetime is managed by this content manager.
    /// </summary>
    /// <param name="disposable"></param>
    public T Add<T>(T disposable) where T : IDisposable
    {
        CheckDisposed();
        _disposables.Add(disposable);
        return disposable;
    }

    /// <summary>
    /// If <see cref="enableHotSwapping"/> is set, checks for file changes of tracked content objects and tries to
    /// hot swap in-place. Otherwise, calling this method does nothing./>
    /// </summary>
    public void UpdateContent() => _contentCache.UpdateCached(this);

    /// <summary>
    /// Disposes all content objects managed by this content manager.
    /// </summary>
    public void Unload()
    {
        foreach (var disposable in _disposables)
            disposable.Dispose();

        _disposables.Clear();
        _contentCache.Clear();
    }

    /// <summary>
    /// Disposes all content objects managed by this content manager, as well as the content manager itself.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            Unload();
            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ContentManager), "The object is already disposed.");
    }

    ~ContentManager() => Dispose();
}