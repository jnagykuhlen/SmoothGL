using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Describes the data types which are valid for shader uniforms.
/// </summary>
public enum ShaderUniformType
{
    Bool = ActiveUniformType.Bool,
    Int = ActiveUniformType.Int,
    UnsignedInt = ActiveUniformType.UnsignedInt,

    Double = ActiveUniformType.Double,
    Double2 = ActiveUniformType.DoubleVec2,
    Double3 = ActiveUniformType.DoubleVec3,
    Double4 = ActiveUniformType.DoubleVec4,

    Float = ActiveUniformType.Float,
    Float2 = ActiveUniformType.FloatVec2,
    Float3 = ActiveUniformType.FloatVec3,
    Float4 = ActiveUniformType.FloatVec4,

    Matrix2 = ActiveUniformType.FloatMat2,
    Matrix3 = ActiveUniformType.FloatMat3,
    Matrix4 = ActiveUniformType.FloatMat4,

    Sampler1D = ActiveUniformType.Sampler1D,
    Sampler2D = ActiveUniformType.Sampler2D,
    Sampler3D = ActiveUniformType.Sampler3D,
    SamplerCube = ActiveUniformType.SamplerCube
}