using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;
using SmoothGL.Content.Internal;
using SmoothGL.Graphics.Shader;
using SmoothGL.Graphics.Texturing;

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

    [JsonPropertyName("uniformAssignments")]
    public IReadOnlyDictionary<string, JsonElement> UniformAssignments { get; set; } = new Dictionary<string, JsonElement>();

    /// <summary>
    /// Creates a shader program from the individual shaders loaded from the corresponding files.
    /// </summary>
    /// <param name="contentProvider">Content provider used to load the shader files.</param>
    /// <returns>Shader program.</returns>
    public ShaderProgram Create(IContentProvider contentProvider)
    {
        var shaderProgram = CompileShaderProgram(contentProvider);

        try
        {
            AssignUniforms(shaderProgram, contentProvider);
        }
        catch (Exception)
        {
            shaderProgram.Dispose();
            throw;
        }

        return shaderProgram;
    }

    private ShaderProgram CompileShaderProgram(IContentProvider contentProvider)
    {
        try
        {
            var vertexShaderCode = LoadShaderCode(contentProvider, VertexShaderFilePath);
            var tessellationControlShaderCode = TryLoadShaderCode(contentProvider, TessellationControlFilePath);
            var tessellationEvaluationShaderCode = TryLoadShaderCode(contentProvider, TessellationEvaluationFilePath);
            var geometryShaderCode = TryLoadShaderCode(contentProvider, GeometryShaderFilePath);
            var fragmentShaderCode = LoadShaderCode(contentProvider, FragmentShaderFilePath);

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

    private static string? TryLoadShaderCode(IContentProvider contentProvider, string? filePath) =>
        filePath == null ? null : LoadShaderCode(contentProvider, filePath);

    private static string LoadShaderCode(IContentProvider contentProvider, string filePath)
    {
        var shaderCode = contentProvider.Load<string>(filePath);
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

            return contentProvider.Load<string>(includePath);
        });
    }

    private void AssignUniforms(ShaderProgram shaderProgram, IContentProvider contentProvider)
    {
        foreach (var (uniformName, uniformValue) in UniformAssignments)
        {
            var uniform = shaderProgram.Uniform(uniformName);
            if (uniform != null)
                AssignUniform(uniform, uniformValue, contentProvider);
        }
    }

    private static void AssignUniform(ShaderUniform uniform, JsonElement uniformValue, IContentProvider contentProvider)
    {
        uniform.SetValue(uniform.Type switch
        {
            ShaderUniformType.Bool => uniformValue.GetBoolean(),
            ShaderUniformType.Int => uniformValue.GetInt32(),
            ShaderUniformType.Float => uniformValue.GetSingle(),
            ShaderUniformType.Float2 => uniformValue.Deserialize<Vector2>(CommonJsonSerializerOptions.Default),
            ShaderUniformType.Float3 => uniformValue.Deserialize<Vector3>(CommonJsonSerializerOptions.Default),
            ShaderUniformType.Float4 => uniformValue.Deserialize<Vector4>(CommonJsonSerializerOptions.Default),
            ShaderUniformType.Matrix2 => uniformValue.Deserialize<Matrix2>(CommonJsonSerializerOptions.Default),
            ShaderUniformType.Matrix3 => uniformValue.Deserialize<Matrix3>(CommonJsonSerializerOptions.Default),
            ShaderUniformType.Matrix4 => uniformValue.Deserialize<Matrix4>(CommonJsonSerializerOptions.Default),
            ShaderUniformType.Sampler2D => contentProvider.Load<Texture2D>(uniformValue.GetString() ?? throw new JsonException("Value is not a path string.")),
            ShaderUniformType.SamplerCube => contentProvider.Load<TextureCube>(uniformValue.GetString() ?? throw new JsonException("Value is not a path string.")),
            _ => throw new JsonException($"Uniform assignment from JSON is not supported for type {uniform.Type}.")
        });
    }
}