namespace SmoothGL.Content.Internal;

public class ContentReaders
{
    private readonly Dictionary<Type, object> _contentReaders = new();

    public void SetContentReader<T>(IContentReader<T> contentReader) where T : notnull
    {
        _contentReaders[typeof(T)] = contentReader;
    }

    public IContentReader<T> GetContentReader<T>() where T : notnull
    {
        var requestedType = typeof(T);
        var type = requestedType;
        do
        {
            if (_contentReaders.GetValueOrDefault(type) is IContentReader<T> contentReader && (contentReader.CanReadSubtypes || type == requestedType))
                return contentReader;

            type = type.BaseType;
        } while (type != null);

        throw new ContentLoadException($"There is no content reader registered for type {requestedType}.", null, null, requestedType);
    }
}