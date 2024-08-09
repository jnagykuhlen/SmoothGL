using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Shader;

public enum ShaderStage
{
    Vertex = ShaderType.VertexShader,
    TessellationControl = ShaderType.TessControlShader,
    TessellationEvaluation = ShaderType.TessEvaluationShader,
    Geometry = ShaderType.GeometryShader,
    Fragment = ShaderType.FragmentShader
}