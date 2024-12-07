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
public class ContentManager(string rootPath) : IContentProvider, IDisposable
{
    private readonly Dictionary<Type, IContentReader<object>> _contentReaders = new();
    private readonly Dictionary<(Type, NormalizedPath), object> _cachedObjects = new();
    private readonly List<IDisposable> _disposables = new();
    private bool _disposed;

    /// <summary>
    /// Creates a new content manager with readers for default content types already registered.
    /// </summary>
    /// <param name="rootPath">Root directory the paths of content files are relative to.</param>
    /// <returns>Content manager.</returns>
    public static ContentManager CreateDefault(string rootPath)
    {
        var contentManager = new ContentManager(rootPath);
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
        try
        {
            if (_cachedObjects.TryGetValue((typeof(T), filePath), out var cachedObject))
                return (T)cachedObject;

            using var fileStream = File.OpenRead(filePath);
            var newObject = Load<T>(fileStream);

            _cachedObjects[(typeof(T), filePath)] = newObject;

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

        var newObject = Read<T>(stream);

        if (newObject is IDisposable disposable)
            _disposables.Add(disposable);

        return newObject;
    }

    private T Read<T>(Stream stream) where T : notnull
    {
        var requestedType = typeof(T);
        var type = requestedType;
        do
        {
            if (_contentReaders.TryGetValue(type, out var contentReader) && (contentReader.CanReadSubtypes || type == requestedType))
                return contentReader.Read<T>(stream, this);

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

    /// <summary>
    /// Disposes all content objects managed by this content manager.
    /// </summary>
    public void Unload()
    {
        foreach (var disposable in _disposables)
            disposable.Dispose();

        _disposables.Clear();
        _cachedObjects.Clear();
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

    ~ContentManager()
    {
        Dispose();
    }
}