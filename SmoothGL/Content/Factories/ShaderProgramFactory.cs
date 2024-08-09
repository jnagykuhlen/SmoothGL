using System.Xml.Serialization;
using SmoothGL.Graphics.Shader;

namespace SmoothGL.Content.Factories;

/// <summary>
/// Serializable, mutable factory which creates shader programs.
/// </summary>
[XmlRoot(ElementName = "ShaderProgram")]
public class ShaderProgramFactory : IFactory<ShaderProgram>
{
    private const string IncludeToken = "#include";

    /// <summary>
    /// Path to the vertex shader code file.
    /// </summary>
    [XmlElement(ElementName = "Vertex")]
    public string? VertexShaderFilePath { get; set; }

    /// <summary>
    /// Path to the tessellation control shader code file.
    /// Set this property to null if the tessellation control shader is not required.
    /// </summary>
    [XmlElement(ElementName = "TessellationControl")]
    public string? TessellationControlFilePath { get; set; }

    /// <summary>
    /// Path to the tessellation evaluation shader code file.
    /// Set this property to null if the tessellation evaluation shader is not required.
    /// </summary>
    [XmlElement(ElementName = "TessellationEvaluation")]
    public string? TessellationEvaluationFilePath { get; set; }

    /// <summary>
    /// Path to the geometry shader code file.
    /// Set this property to null if the geometry shader is not required.
    /// </summary>
    [XmlElement(ElementName = "Geometry")]
    public string? GeometryShaderFilePath { get; set; }

    /// <summary>
    /// Path to the fragment shader code file.
    /// </summary>
    [XmlElement(ElementName = "Fragment")]
    public string? FragmentShaderFilePath { get; set; }

    /// <summary>
    /// Creates a shader program from the individual shaders loaded from the corresponding files.
    /// </summary>
    /// <param name="contentManager">Content manager used to load the shader files.</param>
    /// <returns>Shader program.</returns>
    public ShaderProgram Create(ContentManager contentManager)
    {
        try
        {
            var vertexShaderCode = TryLoadShaderCode(contentManager, VertexShaderFilePath) ??
                                   throw new ShaderCompilationException("Vertex shader file must be provided.", ShaderStage.Vertex);

            var tessellationControlShaderCode = TryLoadShaderCode(contentManager, TessellationControlFilePath);
            var tessellationEvaluationShaderCode = TryLoadShaderCode(contentManager, TessellationEvaluationFilePath);
            var geometryShaderCode = TryLoadShaderCode(contentManager, GeometryShaderFilePath);

            var fragmentShaderCode = TryLoadShaderCode(contentManager, FragmentShaderFilePath) ??
                                     throw new ShaderCompilationException("Fragment shader file must be provided.", ShaderStage.Fragment);

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
                _ => "<unknown shader file>"
            };

            throw new ShaderCompilationException($"Shader compilation error in file {filePath}: {exception.Message}", exception.ShaderStage, exception.ShaderCode);
        }
    }

    private static string? TryLoadShaderCode(ContentManager contentManager, string? filePath) =>
        filePath == null ? null : LoadShaderCode(contentManager, filePath);

    private static string LoadShaderCode(ContentManager contentManager, string filePath)
    {
        var shaderCode = contentManager.Load<string>(filePath);

        var filesIncluded = new HashSet<string>(PathEqualityComparer.Instance) { filePath };
        var baseDirectory = Path.GetDirectoryName(filePath) ?? "";

        var index = 0;
        while ((index = shaderCode.IndexOf(IncludeToken, index, StringComparison.InvariantCulture)) >= 0)
        {
            var endIndex = shaderCode.IndexOfAny(['\n', '\r'], index);
            if (endIndex < 0)
                endIndex = shaderCode.Length;

            var includeCode = "";

            var includeArgumentIndex = index + IncludeToken.Length;
            var includeArgument = shaderCode.Substring(includeArgumentIndex, endIndex - includeArgumentIndex).Trim();

            if (includeArgument[0] == '\"' && includeArgument[^1] == '\"')
            {
                var relativeIncludePath = includeArgument[1..^1];
                var includePath = Path.Combine(baseDirectory, relativeIncludePath);

                if (!filesIncluded.Contains(includePath))
                {
                    includeCode = contentManager.Load<string>(includePath);
                    filesIncluded.Add(includePath);
                }
            }
            else
            {
                includeCode = "#error Include path must be enclosed by quotation marks.";
            }

            shaderCode = shaderCode.Remove(index, endIndex - index).Insert(index, includeCode);
        }

        return shaderCode;
    }
    
    private class PathEqualityComparer : IEqualityComparer<string>
    {
        public static readonly PathEqualityComparer Instance = new();
    
        public bool Equals(string? first, string? second) =>
            first != null && second != null && string.Equals(NormalizePath(first), NormalizePath(second), StringComparison.InvariantCultureIgnoreCase);
        
        public int GetHashCode(string value) => NormalizePath(value).GetHashCode();
    
        private static string NormalizePath(string path) =>
            Path.GetFullPath(path).TrimEnd('\\');
    }
}