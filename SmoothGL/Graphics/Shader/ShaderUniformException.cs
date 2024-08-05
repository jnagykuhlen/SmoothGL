namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Exception which is thrown when an invalid operation is performed on shader uniforms.
/// </summary>
public class ShaderUniformException : Exception
{
    /// <summary>
    /// Creates a new ShaderUniformException.
    /// </summary>
    /// <param name="message">Message specifying the reason for the exception.</param>
    /// <param name="uniformName">Name of the shader uniform which caused the exception.</param>
    /// <param name="uniformType">Type of the shader uniform which caused the exception.</param>
    public ShaderUniformException(string message, string uniformName, ShaderUniformType uniformType)
        : base(message)
    {
        UniformName = uniformName;
        UniformType = uniformType;
    }

    /// <summary>
    /// Gets the name of the shader uniform which caused the exception.
    /// </summary>
    public string UniformName { get; }

    /// <summary>
    /// Gets the type of the shader uniform which caused the exception.
    /// </summary>
    public ShaderUniformType UniformType { get; }
}