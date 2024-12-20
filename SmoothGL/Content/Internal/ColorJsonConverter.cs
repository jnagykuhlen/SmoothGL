using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace SmoothGL.Content.Internal;

public class ColorJsonConverter : JsonConverter<Color4>
{
    public override Color4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var colorString = reader.GetString()?.TrimStart('#') ?? throw new JsonException("Expected non-null color string.");
        return colorString.Length switch
        {
            6 => ParseRgb(colorString),
            8 => ParseRgba(colorString),
            _ => throw new JsonException("Color string is neither RGB nor RGBA format.")
        };
    }
    
    private static Color4 ParseRgb(string colorString) => new(
        ParseColorComponent(colorString, 0), 
        ParseColorComponent(colorString, 2), 
        ParseColorComponent(colorString, 4), 
        byte.MaxValue
    );
    
    private static Color4 ParseRgba(string colorString) => new(
        ParseColorComponent(colorString, 0), 
        ParseColorComponent(colorString, 2), 
        ParseColorComponent(colorString, 4), 
        ParseColorComponent(colorString, 6)
    );

    private static byte ParseColorComponent(string colorString, int index) =>
        byte.Parse(colorString.Substring(index, 2), NumberStyles.HexNumber);

    public override void Write(Utf8JsonWriter writer, Color4 value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"Serialization of type {nameof(Color4)} is not supported.");
}