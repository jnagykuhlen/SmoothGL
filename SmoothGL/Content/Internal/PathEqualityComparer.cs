namespace SmoothGL.Content.Internal;

public class PathEqualityComparer : IEqualityComparer<string>
{
    public static readonly PathEqualityComparer Instance = new();

    public bool Equals(string? first, string? second) =>
        first != null && second != null && string.Equals(NormalizePath(first), NormalizePath(second), StringComparison.InvariantCultureIgnoreCase);

    public int GetHashCode(string value) => NormalizePath(value).GetHashCode();

    private static string NormalizePath(string path) =>
        Path.GetFullPath(path).TrimEnd('\\');
}