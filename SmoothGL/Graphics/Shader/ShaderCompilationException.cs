using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Exception which is thrown when a shader fails to compile.
/// </summary>
public class ShaderCompilationException : Exception
{
    /// <summary>
    /// Creates a new ShaderCompilationException.
    /// </summary>
    /// <param name="message">Message specifying why the shader does not compile.</param>
    /// <param name="shaderStage">Shader stage which does not compile.</param>
    /// <param name="shaderCode">Incorrect shader code causing the compilation error.</param>
    public ShaderCompilationException(string message, ShaderStage shaderStage, string? shaderCode = null)
        : base(message)
    {
        ShaderStage = shaderStage;
        ShaderCode = shaderCode;
    }

    /// <summary>
    /// Gets the shader stage which does not compile.
    /// </summary>
    public ShaderStage ShaderStage { get; }

    /// <summary>
    /// Gets the incorrect shader code causing the compilation error.
    /// </summary>
    public string? ShaderCode { get; }
}