using System.Text.Json;

namespace SmoothGL.Content.Internal;

public static class CommonJsonSerializerOptions
{
    public static readonly JsonSerializerOptions CaseInsensitive = new() { PropertyNameCaseInsensitive = true };
}