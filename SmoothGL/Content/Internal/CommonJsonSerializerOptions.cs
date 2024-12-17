using System.Text.Json;
using OpenTK.Mathematics;

namespace SmoothGL.Content.Internal;

public static class CommonJsonSerializerOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new NonScalarJsonConverter<Vector2>(2, values => new Vector2(values[0], values[1])),
            new NonScalarJsonConverter<Vector3>(3, values => new Vector3(values[0], values[1], values[2])),
            new NonScalarJsonConverter<Vector4>(4, values => new Vector4(values[0], values[1], values[2], values[3])),
            new NonScalarJsonConverter<Matrix2>(4, values => new Matrix2(values[0], values[1], values[2], values[3])),
            new NonScalarJsonConverter<Matrix3>(9, values => new Matrix3(
                values[0], values[1], values[2],
                values[3], values[4], values[5],
                values[6], values[7], values[8]
            )),
            new NonScalarJsonConverter<Matrix4>(16, values => new Matrix4(
                values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7],
                values[8], values[9], values[10], values[11],
                values[12], values[13], values[14], values[15]
            ))
        }
    };
}