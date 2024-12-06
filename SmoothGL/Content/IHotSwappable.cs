namespace SmoothGL.Content;

public interface IHotSwappable<in T>
{
    void HotSwap(T other);
}