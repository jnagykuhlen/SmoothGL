namespace SmoothGL.Content.Internal;

public class ContentReaders
{
    private readonly Dictionary<Type, IContentReader<object>> _contentReaders = new();

    public void SetContentReader<T>(IContentReader<T> contentReader) where T : notnull
    {
        _contentReaders[typeof(T)] = (IContentReader<object>)contentReader;
    }

    public IContentReader<T> GetContentReader<T>() where T : notnull
    {
        var requestedType = typeof(T);
        var type = requestedType;
        do
        {
            if (_contentReaders.TryGetValue(type, out var contentReader) && (contentReader.CanReadSubtypes || type == requestedType))
                return contentReader as IContentReader<T> ?? new TypedContentReader<T>(contentReader);

            type = type.BaseType;
        } while (type != null);

        throw new ContentLoadException($"There is no content reader registered for type {requestedType}.", null, null, requestedType);
    }

    private class TypedContentReader<T>(IContentReader<object> inner) : IContentReader<T> where T : notnull
    {
        public bool CanReadSubtypes => inner.CanReadSubtypes;
        public T Read(Stream stream, Type requestedType, IContentProvider contentProvider) =>
            (T)inner.Read(stream, requestedType, contentProvider);
    }
}