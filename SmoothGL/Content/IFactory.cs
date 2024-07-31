namespace SmoothGL.Content;

/// <summary>
///     Represents an object which is capable of creating instances of the specified product type.
/// </summary>
/// <typeparam name="T">Type of the products created by this factory.</typeparam>
public interface IFactory<out T>
{
    /// <summary>
    ///     Creates a product of the specified type.
    /// </summary>
    /// <param name="contentManager">Content manager used to load additional data.</param>
    /// <returns>Product.</returns>
    T Create(ContentManager contentManager);
}