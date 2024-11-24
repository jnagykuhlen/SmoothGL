namespace SmoothGL.Content;

/// <summary>
/// Reader class which does not read content from a stream directly, but
/// loads a factory for this type and uses it to create the content object.
/// </summary>
/// <typeparam name="TProduct">Content type read by this reader.</typeparam>
/// <typeparam name="TFactory">Type of factory which is capable of creating the requested content type.</typeparam>
public class FactoryReader<TProduct, TFactory> : IContentReader<TProduct>
    where TFactory : IFactory<TProduct>
    where TProduct : notnull
{
    /// <summary>
    /// Reads a content object from a stream by loading a factory which then creates the requested product.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentProvider">Content provider used to load additional data.</param>
    /// <returns>The read object.</returns>
    public TProduct Read(Stream stream, Type requestedType, IContentProvider contentProvider)
    {
        var factory = contentProvider.Load<TFactory>(stream);
        return factory.Create(contentProvider);
    }

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;
}