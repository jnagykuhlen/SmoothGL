namespace SmoothGL.Content.Internal;

public class CachedResult
{
    public CachedResult(object value, bool isNew)
    {
        Value = value;
        IsNew = isNew;
    }

    public object Value { get; }

    public bool IsNew { get; }
}