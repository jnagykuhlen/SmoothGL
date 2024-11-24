namespace SmoothGL.Content.Internal;

public class NormalizedPath(string path)
{
    private string Path { get; } = Normalize(path);

    public override bool Equals(object? other) =>
        other is NormalizedPath otherNormalizedPath && otherNormalizedPath.Path == Path;

    public override int GetHashCode() => Path.GetHashCode();
    public override string ToString() => Path;

    public static implicit operator string(NormalizedPath normalizedPath) => normalizedPath.Path;
    public static implicit operator NormalizedPath(string filePath) => new(filePath);

    private static string Normalize(string filePath) =>
        filePath.Replace('\\', '/').Trim('/').ToLowerInvariant();
}