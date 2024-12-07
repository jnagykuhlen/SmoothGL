using SmoothGL.Content.Factories;
using SmoothGL.Content.Internal;
using SmoothGL.Content.Readers;
using SmoothGL.Graphics.Shader;
using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Content;

/// <summary>
/// Handles loading of content from files or streams and takes care of disposing loaded content automatically.
/// <param name="rootPath">Root directory the paths of content files are relative to.</param>
/// </summary>
public class HotSwappingContentManager(string rootPath) : IContentProvider, IDisposable
{
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    private readonly ContentReaders _contentReaders = new();
    private readonly Dictionary<LoadingSource, HotSwappableNode> _hotSwappableNodes = new();
    private readonly List<IDisposable> _disposables = new();
    private DateTime _lastUpdateTime = DateTime.Now;
    private bool _disposed;

    /// <summary>
    /// Creates a new content manager with readers for default content types already registered.
    /// </summary>
    /// <param name="rootPath">Root directory the paths of content files are relative to.</param>
    /// <returns>Content manager.</returns>
    public static HotSwappingContentManager CreateDefault(string rootPath)
    {
        var contentManager = new HotSwappingContentManager(rootPath);
        contentManager.SetContentReader(new SerializationReader());
        contentManager.SetContentReader(new Readers.StringReader());
        contentManager.SetContentReader(new ImageDataReader());
        contentManager.SetContentReader(new TextureCubeReader(TextureFilterMode.Default));
        contentManager.SetContentReader(new FactoryReader<ShaderProgram, ShaderProgramFactory>());
        contentManager.SetContentReader(new WavefrontObjReader());
        contentManager.SetContentReader(new VertexArrayReader());
        contentManager.SetContentReader(new ColorTextureReader(TextureFilterMode.Default));
        return contentManager;
    }

    /// <summary>
    /// Registers a content reader which handles loading of content of the specified type.
    /// When a reader is already registered for this type, it will be replaced. The content manager can be
    /// allowed to cache created content objects of this type to reduce loading times when the same file is
    /// requested multiple times. However, caching can cause problems when content objects are not immutable.
    /// </summary>
    /// <typeparam name="T">Content type which can be loaded by the reader.</typeparam>
    /// <param name="contentReader">Content reader of the specified type.</param>
    public void SetContentReader<T>(IContentReader<T> contentReader) where T : notnull =>
        _contentReaders.SetContentReader(contentReader);

    /// <summary>
    /// Loads content from a file.
    /// </summary>
    /// <typeparam name="T">The requested content type to load.</typeparam>
    /// <param name="relativeFilePath">Relative path to the file storing content data.</param>
    /// <returns>Content object.</returns>
    public T Load<T>(string relativeFilePath) where T : notnull
    {
        CheckDisposed();

        var filePath = FullFilePath(relativeFilePath);
        var loadingSource = new LoadingSource(typeof(T), relativeFilePath);

        try
        {
            if (_hotSwappableNodes.TryGetValue(loadingSource, out var hotSwappableNode))
                return (T)hotSwappableNode.ContentObject;

            var contentProviderProxy = new ContentProviderProxy(this);
            var contentReader = _contentReaders.GetContentReader<T>();

            using var fileStream = File.OpenRead(filePath);
            var contentObject = contentReader.Read<T>(fileStream, contentProviderProxy);

            var hotSwapAction = CreateHotSwapAction(contentObject, contentReader);
            if (hotSwapAction != null)
                _hotSwappableNodes[loadingSource] = new HotSwappableNode(contentObject, contentProviderProxy.Dependencies, hotSwapAction);

            if (contentObject is IDisposable disposable)
                _disposables.Add(disposable);

            return contentObject;
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            throw new ContentLoadException($"Cannot find content file {filePath}.", fileNotFoundException, filePath, typeof(T));
        }
        catch (Exception exception)
        {
            throw new ContentLoadException($"Unable to load content file {filePath}:\n{exception.Message}", exception, filePath, typeof(T));
        }
    }

    private static Action<Stream, IContentProvider>? CreateHotSwapAction<T>(T contentObject, IContentReader<T> contentReader) where T : notnull =>
        (contentObject, contentReader) switch
        {
            (IHotSwappable<T> hotSwappable, _) => (stream, contentProvider) => hotSwappable.HotSwap(contentReader.Read<T>(stream, contentProvider)),
            (_, IHotSwappingContentReader<T> hotSwappingContentReader) => (stream, contentProvider) => hotSwappingContentReader.ReadInto(contentObject, stream, contentProvider),
            _ => null
        };

    /// <summary>
    /// Loads content from a stream.
    /// </summary>
    /// <typeparam name="T">The requested content type to load.</typeparam>
    /// <param name="stream">Stream from which content data is read.</param>
    /// <returns>Content object.</returns>
    public T Load<T>(Stream stream) where T : notnull
    {
        CheckDisposed();

        var contentObject = _contentReaders.GetContentReader<T>().Read<T>(stream, this);
        if (contentObject is IDisposable disposable)
            _disposables.Add(disposable);

        return contentObject;
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

    public void UpdateContent()
    {
        var currentUpdateTime = DateTime.Now;
        if (currentUpdateTime >= _lastUpdateTime + UpdateInterval)
        {
            CheckForFilesChanged();
            _lastUpdateTime = currentUpdateTime;
        }
    }

    private void CheckForFilesChanged()
    {
        foreach (var (loadingSource, hotSwappableNode) in _hotSwappableNodes)
        {
            var selfOrDependencyChanged = hotSwappableNode.Dependencies
                .Where(dependency => !_hotSwappableNodes.ContainsKey(dependency))
                .Select(dependency => dependency.RelativeFilePath)
                .Append(loadingSource.RelativeFilePath)
                .Any(FileChanged);

            if (selfOrDependencyChanged)
                HotSwap(hotSwappableNode, loadingSource);
        }
    }

    private bool FileChanged(NormalizedPath relativeFilePath) =>
        File.GetLastWriteTime(FullFilePath(relativeFilePath)) > _lastUpdateTime;

    private void HotSwap(HotSwappableNode hotSwappableNode, LoadingSource loadingSource)
    {
        try
        {
            var contentProviderProxy = new ContentProviderProxy(this);
            using var fileStream = File.OpenRead(FullFilePath(loadingSource.RelativeFilePath));

            hotSwappableNode.HotSwap(fileStream, contentProviderProxy);
            hotSwappableNode.Dependencies = contentProviderProxy.Dependencies;

            Console.WriteLine($"Hot swap for '{loadingSource.RelativeFilePath}' successful");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Hot swap for '{loadingSource.RelativeFilePath}' failed: {exception.Message}");
        }
    }

    private string FullFilePath(string relativeFilePath) => Path.Combine(rootPath, relativeFilePath);

    /// <summary>
    /// Disposes all content objects managed by this content manager.
    /// </summary>
    public void Unload()
    {
        foreach (var disposable in _disposables)
            disposable.Dispose();

        _disposables.Clear();
        _hotSwappableNodes.Clear();
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

    ~HotSwappingContentManager() => Dispose();

    private class ContentProviderProxy(IContentProvider innerProvider) : IContentProvider
    {
        public HashSet<LoadingSource> Dependencies { get; } = new();

        public T Load<T>(string relativeFilePath) where T : notnull
        {
            Dependencies.Add(new LoadingSource(typeof(T), relativeFilePath));
            return innerProvider.Load<T>(relativeFilePath);
        }

        public T Load<T>(Stream stream) where T : notnull => innerProvider.Load<T>(stream);
        public T Add<T>(T disposable) where T : IDisposable => innerProvider.Add(disposable);
    }

    private class HotSwappableNode(object contentObject, IReadOnlySet<LoadingSource> dependencies, Action<Stream, IContentProvider> hotSwapAction)
    {
        public object ContentObject { get; } = contentObject;
        public IReadOnlySet<LoadingSource> Dependencies { get; set; } = dependencies;
        public void HotSwap(Stream stream, IContentProvider contentProvider) => hotSwapAction(stream, contentProvider);
    }

    private record LoadingSource(Type Type, NormalizedPath RelativeFilePath);
}