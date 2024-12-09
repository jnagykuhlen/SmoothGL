namespace SmoothGL.Content.Internal;

public class HotSwappingContentCache(ContentDirectory contentDirectory) : IContentCache
{
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    private readonly Dictionary<CachingSource, HotSwappableNode> _hotSwappableNodes = new();
    private DateTime _lastUpdateTime = DateTime.Now;

    public T? GetCached<T>(string relativeFilePath) where T : class =>
        _hotSwappableNodes.GetValueOrDefault(new CachingSource(typeof(T), relativeFilePath))?.ContentObject as T;

    public T AddToCache<T>(string relativeFilePath, IContentReader<T> contentReader, IContentProvider contentProvider) where T : class
    {
        var contentProviderProxy = new ContentProviderProxy(contentProvider);
        using var fileStream = contentDirectory.OpenRead(relativeFilePath);
        var contentObject = contentReader.Read<T>(fileStream, contentProviderProxy);

        var hotSwapAction = CreateHotSwapAction(contentObject, contentReader);
        if (hotSwapAction != null)
        {
            var cachingSource = new CachingSource(typeof(T), relativeFilePath);
            _hotSwappableNodes[cachingSource] = new HotSwappableNode(contentObject, contentProviderProxy.Dependencies, hotSwapAction);
        }

        return contentObject;
    }

    public void UpdateCached(IContentProvider contentProvider)
    {
        var currentUpdateTime = DateTime.Now;
        if (currentUpdateTime >= _lastUpdateTime + UpdateInterval)
        {
            HotSwapOnFileChanged(contentProvider);
            _lastUpdateTime = currentUpdateTime;
        }
    }

    public void Clear() => _hotSwappableNodes.Clear();

    private void HotSwapOnFileChanged(IContentProvider contentProvider)
    {
        foreach (var (cachingSource, hotSwappableNode) in _hotSwappableNodes)
        {
            var fileChanged = hotSwappableNode.Dependencies
                .Where(dependency => !_hotSwappableNodes.ContainsKey(dependency))
                .Select(dependency => dependency.RelativeFilePath)
                .Append(cachingSource.RelativeFilePath)
                .Any(FileChanged);

            if (fileChanged)
                HotSwap(hotSwappableNode, cachingSource.RelativeFilePath, contentProvider);
        }
    }

    private bool FileChanged(NormalizedPath relativeFilePath) =>
        contentDirectory.GetLastWriteTime(relativeFilePath) > _lastUpdateTime;

    private void HotSwap(HotSwappableNode hotSwappableNode, NormalizedPath relativeFilePath, IContentProvider contentProvider)
    {
        try
        {
            var contentProviderProxy = new ContentProviderProxy(contentProvider);
            using var fileStream = contentDirectory.OpenRead(relativeFilePath);

            hotSwappableNode.HotSwap(fileStream, contentProviderProxy);
            hotSwappableNode.Dependencies = contentProviderProxy.Dependencies;

            Console.WriteLine($"Hot swap for '{relativeFilePath}' successful");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Hot swap for '{relativeFilePath}' failed: {exception.Message}");
        }
    }

    private static Action<Stream, IContentProvider>? CreateHotSwapAction<T>(T contentObject, IContentReader<T> contentReader) where T : notnull =>
        (contentObject, contentReader) switch
        {
            (IHotSwappable<T> hotSwappable, _) => (stream, contentProvider) => hotSwappable.HotSwap(contentReader.Read<T>(stream, contentProvider)),
            (_, IHotSwappingContentReader<T> hotSwappingContentReader) => (stream, contentProvider) => hotSwappingContentReader.ReadInto(contentObject, stream, contentProvider),
            _ => null
        };

    private class ContentProviderProxy(IContentProvider innerProvider) : IContentProvider
    {
        private readonly HashSet<CachingSource> _dependencies = new();
        public IReadOnlySet<CachingSource> Dependencies => _dependencies;

        public T Load<T>(string relativeFilePath) where T : class
        {
            _dependencies.Add(new CachingSource(typeof(T), relativeFilePath));
            return innerProvider.Load<T>(relativeFilePath);
        }

        public T Load<T>(Stream stream) where T : class => innerProvider.Load<T>(stream);
        public T Add<T>(T disposable) where T : IDisposable => innerProvider.Add(disposable);
    }

    private class HotSwappableNode(object contentObject, IReadOnlySet<CachingSource> dependencies, Action<Stream, IContentProvider> hotSwapAction)
    {
        public object ContentObject { get; } = contentObject;
        public IReadOnlySet<CachingSource> Dependencies { get; set; } = dependencies;
        public void HotSwap(Stream stream, IContentProvider contentProvider) => hotSwapAction(stream, contentProvider);
    }

    private record CachingSource(Type Type, NormalizedPath RelativeFilePath);
}