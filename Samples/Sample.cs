using OpenTK.Windowing.Desktop;

namespace SmoothGL.Samples;

public interface ISample
{
    string Title { get; }
    GameWindow CreateWindow();
}

public record Sample<T>(string Title) : ISample where T : GameWindow, new()
{
    public GameWindow CreateWindow() => new T();
}
