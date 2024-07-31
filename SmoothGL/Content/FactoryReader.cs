namespace SmoothGL.Content;

/// <summary>
/// Reader class which does not read content from a stream directly, but
/// loads a factory for this type and uses it to create the content object.
/// </summary>
/// <typeparam name="TProduct">Content type read by this reader.</typeparam>
/// <typeparam name="TFactory">Type of a factory which is capable of creating the requested content type.</typeparam>
public class FactoryReader<TProduct, TFactory> : IContentReader<TProduct>
    where TFactory : IFactory<TProduct>
{
    /// <summary>
    /// Reads a content object from a stream by loading a factory which then creates the requested product.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>The read object.</returns>
    public TProduct Read(Stream stream, Type requestedType, ContentManager contentManager)
    {
        IFactory<TProduct> factory = contentManager.Load<TFactory>(stream);
        return factory.Create(contentManager);
    }

    /// <summary>
    /// Indicates whether this class can also read subtypes of the specified type.
    /// </summary>
    public bool CanReadSubtypes => false;

    /// <summary>
    /// Gets the name of this reader.
    /// </summary>
    public string ReaderName => "FactoryReader";
}