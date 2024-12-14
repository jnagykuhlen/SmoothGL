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
    public ShaderUniformException(string message)
        : base(message)
    {
    }
}