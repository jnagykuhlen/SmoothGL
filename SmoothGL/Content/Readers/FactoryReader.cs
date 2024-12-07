namespace SmoothGL.Content.Readers;

/// <summary>
/// Reader class which does not read content from a stream directly, but
/// loads a factory for this type and uses it to create the content object.
/// </summary>
/// <typeparam name="TProduct">Content type read by this reader.</typeparam>
/// <typeparam name="TFactory">Type of factory which is capable of creating the requested content type.</typeparam>
public class FactoryReader<TProduct, TFactory> : ContentReader<TProduct>
    where TFactory : IFactory<TProduct>
    where TProduct : notnull
{
    protected override TProduct Read(Stream stream, IContentProvider contentProvider)
    {
        var factory = contentProvider.Load<TFactory>(stream);
        return factory.Create(contentProvider);
    }
}