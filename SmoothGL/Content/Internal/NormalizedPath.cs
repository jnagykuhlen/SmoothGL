namespace SmoothGL.Content.Internal;

public class NormalizedPath(string filePath)
{
    private string FilePath { get; } = Normalize(filePath);

    public override bool Equals(object? other) =>
        other is NormalizedPath otherNormalizedPath && otherNormalizedPath.FilePath == FilePath;

    public override int GetHashCode() => FilePath.GetHashCode();
    public override string ToString() => FilePath;

    public static implicit operator string(NormalizedPath normalizedPath) => normalizedPath.FilePath;
    public static implicit operator NormalizedPath(string filePath) => new(filePath);

    private static string Normalize(string filePath) =>
        filePath.Replace('\\', '/').Trim('/').ToLowerInvariant();
}