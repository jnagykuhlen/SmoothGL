namespace SmoothGL.Content;

/// <summary>
/// Content types implementing this interface can be hot swapped in-place with other content of the same type.
/// </summary>
/// <typeparam name="T">Content type to swap with, usually the implementing type.</typeparam>
public interface IHotSwappable<in T>
{
    /// <summary>
    /// Hot swap the contents of the specified object into this object. The other object is expected to remain unchanged.
    /// </summary>
    /// <param name="other">Object that is hot swapped into this instance.</param>
    void HotSwap(T other);
}