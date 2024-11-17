using System.Text.Json.Serialization;
using SmoothGL.Content.Internal;
using SmoothGL.Graphics.Shader;

namespace SmoothGL.Content.Factories;

/// <summary>
/// Serializable factory which creates shader programs.
/// </summary>
public class ShaderProgramFactory : IFactory<ShaderProgram>
{
    private const string IncludeToken = "#include";

    /// <summary>
    /// Path to the vertex shader code file.
    /// </summary>
    [JsonPropertyName("vertex"), JsonRequired]
    public required string VertexShaderFilePath { get; set; }

    /// <summary>
    /// Path to the tessellation control shader code file.
    /// Set this property to null if the tessellation control shader is not required.
    /// </summary>
    [JsonPropertyName("tessellationControl")]
    public string? TessellationControlFilePath { get; set; }

    /// <summary>
    /// Path to the tessellation evaluation shader code file.
    /// Set this property to null if the tessellation evaluation shader is not required.
    /// </summary>
    [JsonPropertyName("tessellationEvaluation")]
    public string? TessellationEvaluationFilePath { get; set; }

    /// <summary>
    /// Path to the geometry shader code file.
    /// Set this property to null if the geometry shader is not required.
    /// </summary>
    [JsonPropertyName("geometry")]
    public string? GeometryShaderFilePath { get; set; }

    /// <summary>
    /// Path to the fragment shader code file.
    /// </summary>
    [JsonPropertyName("fragment"), JsonRequired]
    public required string FragmentShaderFilePath { get; set; }

    /// <summary>
    /// Creates a shader program from the individual shaders loaded from the corresponding files.
    /// </summary>
    /// <param name="contentManager">Content manager used to load the shader files.</param>
    /// <returns>Shader program.</returns>
    public ShaderProgram Create(ContentManager contentManager)
    {
        try
        {
            var vertexShaderCode = LoadShaderCode(contentManager, VertexShaderFilePath);
            var tessellationControlShaderCode = TryLoadShaderCode(contentManager, TessellationControlFilePath);
            var tessellationEvaluationShaderCode = TryLoadShaderCode(contentManager, TessellationEvaluationFilePath);
            var geometryShaderCode = TryLoadShaderCode(contentManager, GeometryShaderFilePath);
            var fragmentShaderCode = LoadShaderCode(contentManager, FragmentShaderFilePath);

            return new ShaderProgram(
                vertexShaderCode,
                tessellationControlShaderCode,
                tessellationEvaluationShaderCode,
                geometryShaderCode,
                fragmentShaderCode
            );
        }
        catch (ShaderCompilationException exception)
        {
            var filePath = exception.ShaderStage switch
            {
                ShaderStage.Vertex => VertexShaderFilePath,
                ShaderStage.TessellationControl => TessellationControlFilePath,
                ShaderStage.TessellationEvaluation => TessellationEvaluationFilePath,
                ShaderStage.Geometry => GeometryShaderFilePath,
                ShaderStage.Fragment => FragmentShaderFilePath,
                _ => "<unknown>"
            };

            throw new ShaderCompilationException($"Shader compilation error in file {filePath}: {exception.Message}", exception.ShaderStage, exception.ShaderCode);
        }
    }

    private static string? TryLoadShaderCode(ContentManager contentManager, string? filePath) =>
        filePath == null ? null : LoadShaderCode(contentManager, filePath);

    private static string LoadShaderCode(ContentManager contentManager, string filePath)
    {
        var shaderCode = contentManager.Load<string>(filePath);
        var filesIncluded = new HashSet<NormalizedPath> { filePath };
        var baseDirectory = Path.GetDirectoryName(filePath) ?? "";

        return StringReplace.ReplaceRecursive(shaderCode, IncludeToken, argument =>
        {
            if (argument[0] != '\"' || argument[^1] != '\"')
                return "#error Include path must be enclosed by quotation marks.";

            var relativeIncludePath = argument[1..^1];
            var includePath = Path.Combine(baseDirectory, relativeIncludePath);

            if (!filesIncluded.Add(includePath))
                return "";

            return contentManager.Load<string>(includePath);
        });
    }
}