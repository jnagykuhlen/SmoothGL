using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
///     Exception which is thrown when a shader fails to compile.
/// </summary>
public class ShaderCompilationException : Exception
{
    /// <summary>
    ///     Creates a new ShaderCompilationException.
    /// </summary>
    /// <param name="message">Message specifying why the shader does not compile.</param>
    /// <param name="shaderType">Type of the shader which does not compile.</param>
    public ShaderCompilationException(string message, ShaderType shaderType)
        : this(message, shaderType, null)
    {
    }

    /// <summary>
    ///     Creates a new ShaderCompilationException.
    /// </summary>
    /// <param name="message">Message specifying why the shader does not compile.</param>
    /// <param name="shaderType">Type of the shader which does not compile.</param>
    /// <param name="shaderCode">Incorrect shader code causing the compilation error.</param>
    public ShaderCompilationException(string message, ShaderType shaderType, string shaderCode)
        : base(message)
    {
        ShaderType = shaderType;
        ShaderCode = shaderCode;
    }

    /// <summary>
    ///     Gets the type of the shader which does not compile.
    /// </summary>
    public ShaderType ShaderType { get; }

    /// <summary>
    ///     Gets the incorrect shader code causing the compilation error.
    /// </summary>
    public string ShaderCode { get; }
}