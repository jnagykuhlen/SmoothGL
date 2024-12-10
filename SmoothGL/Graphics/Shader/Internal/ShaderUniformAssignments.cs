using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Graphics.Shader.Internal;

public static class ShaderUniformAssignments
{
    private static readonly IShaderUniformAssignment BooleanAssignment = new ShaderUniformBooleanAssignment();
    private static readonly IShaderUniformAssignment DoubleAssigment = new ShaderUniformAssignment<double>(GL.Uniform1);
    private static readonly IShaderUniformAssignment Double2Assigment = new ShaderUniformAssignment<Vector2d>((location, value) => GL.Uniform2(location, value.X, value.Y));
    private static readonly IShaderUniformAssignment Double3Assigment = new ShaderUniformAssignment<Vector3d>((location, value) => GL.Uniform3(location, value.X, value.Y, value.Z));
    private static readonly IShaderUniformAssignment Double4Assigment = new ShaderUniformAssignment<Vector4d>((location, value) => GL.Uniform4(location, value.X, value.Y, value.Z, value.W));
    private static readonly IShaderUniformAssignment FloatAssigment = new ShaderUniformAssignment<float>(GL.Uniform1);
    private static readonly IShaderUniformAssignment Float2Assigment = new ShaderUniformAssignment<Vector2>(GL.Uniform2);
    private static readonly IShaderUniformAssignment Float3Assigment = new ShaderUniformAssignment<Vector3>(GL.Uniform3);
    private static readonly IShaderUniformAssignment Float4Assigment = new ShaderUniformAssignment<Vector4>(GL.Uniform4);
    private static readonly IShaderUniformAssignment IntAssigment = new ShaderUniformAssignment<int>(GL.Uniform1);
    private static readonly IShaderUniformAssignment UnsignedIntAssigment = new ShaderUniformAssignment<uint>(GL.Uniform1);
    private static readonly IShaderUniformAssignment Matrix2Assigment = new ShaderUniformAssignment<Matrix2>((location, value) => GL.UniformMatrix2(location, false, ref value));
    private static readonly IShaderUniformAssignment Matrix3Assigment = new ShaderUniformAssignment<Matrix3>((location, value) => GL.UniformMatrix3(location, false, ref value));
    private static readonly IShaderUniformAssignment Matrix4Assigment = new ShaderUniformAssignment<Matrix4>((location, value) => GL.UniformMatrix4(location, false, ref value));
    private static readonly IShaderUniformAssignment Sampler1DAssigment = new ShaderUniformTextureAssignment(value => value is Texture1D);
    private static readonly IShaderUniformAssignment Sampler2DAssigment = new ShaderUniformTextureAssignment(value => value is Texture2D or DepthStencilTexture2D);
    private static readonly IShaderUniformAssignment Sampler3DAssigment = new ShaderUniformTextureAssignment(value => value is Texture3D);
    private static readonly IShaderUniformAssignment SamplerCubeAssigment = new ShaderUniformTextureAssignment(value => value is TextureCube);

    public static IShaderUniformAssignment Get(ShaderUniformType type, int size)
    {
        var assignment = type switch
        {
            ShaderUniformType.Bool => BooleanAssignment,
            ShaderUniformType.Double => DoubleAssigment,
            ShaderUniformType.Double2 => Double2Assigment,
            ShaderUniformType.Double3 => Double3Assigment,
            ShaderUniformType.Double4 => Double4Assigment,
            ShaderUniformType.Float => FloatAssigment,
            ShaderUniformType.Float2 => Float2Assigment,
            ShaderUniformType.Float3 => Float3Assigment,
            ShaderUniformType.Float4 => Float4Assigment,
            ShaderUniformType.Int => IntAssigment,
            ShaderUniformType.UnsignedInt => UnsignedIntAssigment,
            ShaderUniformType.Matrix2 => Matrix2Assigment,
            ShaderUniformType.Matrix3 => Matrix3Assigment,
            ShaderUniformType.Matrix4 => Matrix4Assigment,
            ShaderUniformType.Sampler1D => Sampler1DAssigment,
            ShaderUniformType.Sampler2D => Sampler2DAssigment,
            ShaderUniformType.Sampler3D => Sampler3DAssigment,
            ShaderUniformType.SamplerCube => SamplerCubeAssigment,
            _ => throw new ArgumentException($"There is no assignment registered for uniforms of type {type}.", nameof(type))
        };
        
        return size == 1 ? assignment : new ShaderUniformArrayAssignment(assignment, size);
    }
}
