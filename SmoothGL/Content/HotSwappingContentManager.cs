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

    private readonly Dictionary<Type, IContentReader<object>> _contentReaders = new();
    private readonly Dictionary<LoadingSource, IContentNode> _cachedContentNodes = new();
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

        var colorTextureReader = new ColorTextureReader(TextureFilterMode.Default);
        contentManager.SetContentReader<Texture2D>(colorTextureReader);
        contentManager.SetContentReader(colorTextureReader);

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
    public void SetContentReader<T>(IContentReader<T> contentReader) where T : notnull
    {
        _contentReaders[typeof(T)] = (IContentReader<object>)contentReader;
    }

    /// <summary>
    /// Loads content from a file.
    /// </summary>
    /// <typeparam name="T">The requested content type to load.</typeparam>
    /// <param name="relativeFilePath">Relative path to the file storing content data.</param>
    /// <returns>Content object.</returns>
    public T Load<T>(string relativeFilePath) where T : notnull
    {
        CheckDisposed();

        var filePath = Path.Combine(rootPath, relativeFilePath);
        var loadingSource = new LoadingSource(typeof(T), relativeFilePath);

        try
        {
            if (_cachedContentNodes.TryGetValue(loadingSource, out var cachedContentNode))
                return (T)cachedContentNode.ContentObject;

            var contentProviderProxy = new ContentProviderProxy(this);

            using var fileStream = File.OpenRead(filePath);
            var contentReader = GetContentReader(fileStream, typeof(T));

            var newObject = (T)contentReader.Read(fileStream, typeof(T), contentProviderProxy);

            if (newObject is IHotSwappable || contentReader is IHotSwappingReader)
            {
                _cachedContentNodes[loadingSource] = new ContentNode<T>(
                    newObject,
                    contentProviderProxy.Dependencies,
                    contentReader
                );
            }

            if (newObject is IDisposable disposable)
                _disposables.Add(disposable);

            return newObject;
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

    /// <summary>
    /// Loads content from a stream.
    /// </summary>
    /// <typeparam name="T">The requested content type to load.</typeparam>
    /// <param name="stream">Stream from which content data is read.</param>
    /// <returns>Content object.</returns>
    public T Load<T>(Stream stream) where T : notnull
    {
        CheckDisposed();

        var newObject = (T)GetContentReader(stream, typeof(T)).Read(stream, typeof(T), this);

        if (newObject is IDisposable disposable)
            _disposables.Add(disposable);

        return newObject;
    }

    private IContentReader<object> GetContentReader(Stream stream, Type requestedType)
    {
        var type = requestedType;
        do
        {
            if (_contentReaders.TryGetValue(type, out var contentReader) && (contentReader.CanReadSubtypes || type == requestedType))
                return contentReader;

            type = type.BaseType;
        } while (type != null);

        throw new ContentLoadException($"There is no content reader registered for type {requestedType}.", stream, requestedType);
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
        foreach (var (loadingSource, contentNode) in _cachedContentNodes)
        {
            var selfOrDependencyChanged = contentNode.Dependencies
                .Where(dependency => !_cachedContentNodes.ContainsKey(dependency))
                .Select(dependency => dependency.RelativeFilePath)
                .Append(loadingSource.RelativeFilePath)
                .Any(FileChanged);

            if (selfOrDependencyChanged)
                HotSwap(contentNode, loadingSource);
        }
    }

    public void PrintCachedContentNodes()
    {
        Console.WriteLine("\n--- CACHED CONTENT NODES ---");
        foreach (var ((type, relativeFilePath), contentNode) in _cachedContentNodes)
            Console.WriteLine($"[{contentNode.GetHashCode()}] {relativeFilePath} ({type.Name}) with dependencies {string.Join(',', contentNode.Dependencies)}");
        Console.WriteLine("----------------------------\n");
    }

    private bool FileChanged(NormalizedPath relativeFilePath) =>
        File.GetLastWriteTime(Path.Combine(rootPath, relativeFilePath)) > _lastUpdateTime;

    private void HotSwap(IContentNode contentNode, LoadingSource loadingSource)
    {
        try
        {
            var contentProviderProxy = new ContentProviderProxy(this);

            var filePath = Path.Combine(rootPath, loadingSource.RelativeFilePath);
            using var fileStream = File.OpenRead(filePath);

            contentNode.HotSwap(fileStream, contentProviderProxy);

            _cachedContentNodes[loadingSource] = contentNode.WithDependencies(contentProviderProxy.Dependencies);

            Console.WriteLine($"Hot swap for '{loadingSource.RelativeFilePath}' successful");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Hot swap for '{loadingSource.RelativeFilePath}' failed: {exception.Message}");
        }
    }

    /// <summary>
    /// Disposes all content objects managed by this content manager.
    /// </summary>
    public void Unload()
    {
        foreach (var disposable in _disposables)
            disposable.Dispose();

        _disposables.Clear();
        _cachedContentNodes.Clear();
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

    ~HotSwappingContentManager()
    {
        Dispose();
    }

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

    private interface IContentNode
    {
        object ContentObject { get; }
        IReadOnlySet<LoadingSource> Dependencies { get; }
        void HotSwap(Stream stream, IContentProvider contentProvider);
        IContentNode WithDependencies(IReadOnlySet<LoadingSource> dependencies);
    }

    private record ContentNode<T>(object ContentObject, IReadOnlySet<LoadingSource> Dependencies, IContentReader<object> ContentReader) : IContentNode where T : notnull
    {
        public void HotSwap(Stream stream, IContentProvider contentProvider)
        {
            if (ContentReader is IHotSwappingReader hotSwappingReader)
            {
                hotSwappingReader.ReadInto(ContentObject, stream, contentProvider);
            }
            else if (ContentObject is IHotSwappable hotSwappable)
            {
                hotSwappable.HotSwap(ContentReader.Read(stream, typeof(T), contentProvider));
            }
        }

        public IContentNode WithDependencies(IReadOnlySet<LoadingSource> dependencies) => this with { Dependencies = dependencies };
    }

    private record LoadingSource(Type Type, NormalizedPath RelativeFilePath);
}