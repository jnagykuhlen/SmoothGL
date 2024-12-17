﻿using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Describes the data types which are valid for shader uniforms.
/// </summary>
public enum ShaderUniformType
{
    Bool = ActiveUniformType.Bool,
    Int = ActiveUniformType.Int,

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

public static class ShaderUniformTypeExtension
{
    public static bool IsSampler(this ShaderUniformType shaderUniformType) =>
        shaderUniformType is >= ShaderUniformType.Sampler1D and <= ShaderUniformType.SamplerCube;
}
