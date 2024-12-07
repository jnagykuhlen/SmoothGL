namespace SmoothGL.Content.Readers;

public abstract class ContentReader<T> : IContentReader<T> where T : notnull
{
    public TRequested Read<TRequested>(Stream stream, IContentProvider contentProvider) where TRequested : T
    {
        var result = Read(stream, contentProvider);
        if (result is TRequested requestedResult)
            return requestedResult;

        throw new InvalidCastException($"Content reader for {typeof(T)} cannot read subtype {typeof(TRequested)}.");
    }

    protected abstract T Read(Stream stream, IContentProvider contentProvider);

    public bool CanReadSubtypes => false;
}