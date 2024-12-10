using OpenTK.Graphics.OpenGL;
using SmoothGL.Content;
using SmoothGL.Graphics.Shader.Internal;

namespace SmoothGL.Graphics.Shader;

/// <summary>
/// Represents a shader program linked from a number of compiled shaders.
/// </summary>
public class ShaderProgram : GraphicsResource, IHotSwappable<ShaderProgram>
{
    private static int currentProgramId;

    private int _programId;
    private Dictionary<string, ShaderProgramUniform> _uniforms = new(StringComparer.Ordinal);
    private Dictionary<string, ShaderUniformBlock> _uniformBlocks = new(StringComparer.Ordinal);

    /// <summary>
    /// Creates a new shader program with vertex and fragment shader stage.
    /// </summary>
    /// <param name="vertexShaderCode">Vertex shader source code.</param>
    /// <param name="fragmentShaderCode">Fragment shader source code.</param>
    public ShaderProgram(string vertexShaderCode, string fragmentShaderCode)
        : this(vertexShaderCode, null, null, null, fragmentShaderCode)
    {
    }

    /// <summary>
    /// Creates a new shader program with vertex, geometry and fragment shader stage.
    /// </summary>
    /// <param name="vertexShaderCode">Vertex shader source code.</param>
    /// <param name="geometryShaderCode">Geometry shader source code.</param>
    /// <param name="fragmentShaderCode">Fragment shader source code.</param>
    public ShaderProgram(string vertexShaderCode, string geometryShaderCode, string fragmentShaderCode)
        : this(vertexShaderCode, null, null, geometryShaderCode, fragmentShaderCode)
    {
    }

    /// <summary>
    /// Creates a new shader program with vertex, tessellation and fragment shader stage.
    /// </summary>
    /// <param name="vertexShaderCode">Vertex shader source code.</param>
    /// <param name="tessellationControlShaderCode">Tessellation control shader source code.</param>
    /// <param name="tessellationEvaluationShaderCode">Tessellation evaluation shader source code.</param>
    /// <param name="fragmentShaderCode">Fragment shader source code.</param>
    public ShaderProgram(string vertexShaderCode,
        string tessellationControlShaderCode,
        string tessellationEvaluationShaderCode,
        string fragmentShaderCode)
        : this(vertexShaderCode, tessellationControlShaderCode, tessellationEvaluationShaderCode, null, fragmentShaderCode)
    {
    }

    /// <summary>
    /// Creates a new shader program with vertex, tessellation, geometry and fragment shader stage.
    /// </summary>
    /// <param name="vertexShaderCode">Vertex shader source code.</param>
    /// <param name="tessellationControlShaderCode">Tessellation control shader source code.</param>
    /// <param name="tessellationEvaluationShaderCode">Tessellation evaluation shader source code.</param>
    /// <param name="geometryShaderCode">Geometry shader source code.</param>
    /// <param name="fragmentShaderCode">Fragment shader source code.</param>
    public ShaderProgram(
        string vertexShaderCode,
        string? tessellationControlShaderCode,
        string? tessellationEvaluationShaderCode,
        string? geometryShaderCode,
        string fragmentShaderCode)
    {
        var shaderIds = new List<int>(5);
        try
        {
            shaderIds.Add(CreateShader(ShaderType.VertexShader, vertexShaderCode));
            
            if (tessellationControlShaderCode != null)
                shaderIds.Add(CreateShader(ShaderType.TessControlShader, tessellationControlShaderCode));
            
            if (tessellationEvaluationShaderCode != null)
                shaderIds.Add(CreateShader(ShaderType.TessEvaluationShader, tessellationEvaluationShaderCode));
            
            if (geometryShaderCode != null)
                shaderIds.Add(CreateShader(ShaderType.GeometryShader, geometryShaderCode));
            
            shaderIds.Add(CreateShader(ShaderType.FragmentShader, fragmentShaderCode));

            _programId = LinkProgram(shaderIds);
        }
        finally
        {
            foreach (var shaderId in shaderIds)
                DeleteShader(shaderId);
        }

        InitializeUniforms();
    }

    /// <summary>
    /// Gets all uniforms defined by this shader program.
    /// </summary>
    public IEnumerable<ShaderUniform> Uniforms => _uniforms.Values;

    /// <summary>
    /// Gets all uniform blocks defined by this shader program.
    /// </summary>
    public IEnumerable<ShaderUniformBlock> UniformBlocks => _uniformBlocks.Values;

    /// <summary>
    /// Gets a value indicating whether this shader program is currently in use for subsequent draw operations,
    /// i.e., its <see cref="Use" /> method was called after using any other shader program.
    /// </summary>
    public bool IsActive => currentProgramId == _programId;

    protected override string ResourceName => "ShaderProgram";

    private static int LinkProgram(IReadOnlyCollection<int> shaderIds)
    {
        var programId = GL.CreateProgram();
        foreach (var shaderId in shaderIds)
            GL.AttachShader(programId, shaderId);

        GL.LinkProgram(programId);
        GL.GetProgram(programId, GetProgramParameterName.LinkStatus, out var linked);

        if (linked == 0)
        {
            var message = GL.GetProgramInfoLog(programId);
            GL.DeleteProgram(programId);
            throw new ShaderProgramLinkException(message);
        }

        return programId;
    }

    private static int CreateShader(ShaderType shaderType, string shaderCode)
    {
        var shaderId = GL.CreateShader(shaderType);
        GL.ShaderSource(shaderId, shaderCode);
        GL.CompileShader(shaderId);
        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out var compiled);

        if (compiled == 0)
        {
            var message = GL.GetShaderInfoLog(shaderId);
            GL.DeleteShader(shaderId);
            throw new ShaderCompilationException(message, (ShaderStage)shaderType, shaderCode);
        }

        return shaderId;
    }

    private static void DeleteShader(int shaderId)
    {
        if (shaderId != 0)
            GL.DeleteShader(shaderId);
    }

    private void InitializeUniforms()
    {
        GL.UseProgram(_programId);

        GL.GetProgram(_programId, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        GL.GetProgram(_programId, GetProgramParameterName.ActiveUniformBlocks, out var numberOfUniformBlocks);

        _uniforms.EnsureCapacity(numberOfUniforms);
        _uniformBlocks.EnsureCapacity(numberOfUniformBlocks);

        var uniformBlockElements = new List<UniformBufferElement>[numberOfUniformBlocks];
        for (var i = 0; i < numberOfUniformBlocks; ++i)
            uniformBlockElements[i] = new List<UniformBufferElement>();

        var textureIndex = 0;
        var maxTextureIndex = GL.GetInteger(GetPName.MaxCombinedTextureImageUnits);

        for (var i = 0; i < numberOfUniforms; ++i)
        {
            var uniformName = GL.GetActiveUniform(_programId, i, out var uniformSize, out var uniformRawType);
            var uniformLocation = GL.GetUniformLocation(_programId, uniformName);
            GL.GetActiveUniforms(_programId, 1, ref i, ActiveUniformParameter.UniformBlockIndex, out var uniformBlockIndex);
            var uniformType = (ShaderUniformType)uniformRawType;

            if (uniformName.EndsWith("[0]"))
                uniformName = uniformName[..^3];

            if (uniformBlockIndex == -1)
            {
                if (!Enum.IsDefined(typeof(ShaderUniformType), uniformType))
                    throw new ShaderUniformException(
                        $"The uniform type {uniformType} specified in the shader for uniform {uniformName} is not supported.",
                        uniformName,
                        uniformType
                    );

                if (uniformType is ShaderUniformType.Sampler1D or ShaderUniformType.Sampler2D or ShaderUniformType.Sampler3D or ShaderUniformType.SamplerCube)
                {
                    if (textureIndex + uniformSize > maxTextureIndex)
                    {
                        throw new ShaderUniformException(
                            $"Texture uniform {uniformName} exceeds the limit of {maxTextureIndex} texture units.",
                            uniformName,
                            uniformType
                        );
                    }

                    var textureIndices = Enumerable.Range(textureIndex, uniformSize).ToArray();

                    GL.Uniform1(uniformLocation, uniformSize, textureIndices);

                    uniformLocation = textureIndex;
                    textureIndex += uniformSize;
                }

                var uniform = new ShaderProgramUniform(uniformName, uniformType, uniformSize, uniformLocation);
                _uniforms.Add(uniformName, uniform);
            }
            else
            {
                GL.GetActiveUniforms(_programId, 1, ref i, ActiveUniformParameter.UniformOffset, out var uniformOffset);
                uniformBlockElements[uniformBlockIndex].Add(new UniformBufferElement(uniformName, uniformType, uniformSize, uniformOffset));
            }
        }

        for (var i = 0; i < numberOfUniformBlocks; ++i)
        {
            var uniformBlockName = GL.GetActiveUniformBlockName(_programId, i);
            GL.GetActiveUniformBlock(_programId, i, ActiveUniformBlockParameter.UniformBlockDataSize, out var uniformBlockSize);
            GL.UniformBlockBinding(_programId, i, i);

            var layout = new UniformBufferLayout(uniformBlockSize, uniformBlockElements[i].ToArray());
            _uniformBlocks.Add(uniformBlockName, new ShaderUniformBlock(uniformBlockName, i, layout));
        }
    }

    /// <summary>
    /// Communicates the uniform values to the GPU and uses this shader program for all
    /// subsequent drawing operations.
    /// </summary>
    public void Use()
    {
        CheckDisposed();

        if (currentProgramId != _programId)
        {
            GL.UseProgram(_programId);
            currentProgramId = _programId;
        }

        foreach (var uniform in _uniforms.Values)
            uniform.Apply();

        foreach (var uniformBlock in UniformBlocks)
            uniformBlock.Buffer?.Bind(uniformBlock.Location);
    }

    /// <summary>
    /// Gets the uniform with the specified name. Returns null if such uniform does not exist.
    /// Note that uniform value changes are not communicated to the GPU until this shader
    /// program's <see cref="Use" /> method is called again.
    /// </summary>
    /// <param name="name">Name of the uniform.</param>
    /// <returns>Uniform.</returns>
    public ShaderUniform? Uniform(string name) => _uniforms.GetValueOrDefault(name);

    /// <summary>
    /// Gets the uniform block with the specified name. Returns null if such uniform block does not exist.
    /// </summary>
    /// <param name="name">Name of the uniform block.</param>
    /// <returns>Uniform block.</returns>
    public ShaderUniformBlock? UniformBlock(string name) => _uniformBlocks.GetValueOrDefault(name);

    protected override void FreeResources()
    {
        if (currentProgramId == _programId)
            currentProgramId = 0;

        if (_programId != 0)
            GL.DeleteProgram(_programId);
    }

    void IHotSwappable<ShaderProgram>.HotSwap(ShaderProgram other)
    {
        foreach (var uniform in Uniforms)
        {
            var value = uniform.Value;
            var otherUniform = other.Uniform(uniform.Name);

            if (value != null && otherUniform != null && otherUniform.Type == uniform.Type && otherUniform.Size == uniform.Size)
                otherUniform.SetValue(value);
        }

        foreach (var uniformBlock in UniformBlocks)
        {
            var buffer = uniformBlock.Buffer;
            var otherUniformBlock = other.UniformBlock(uniformBlock.Name);
            
            if (buffer != null && otherUniformBlock != null)
                otherUniformBlock.SetBuffer(buffer);
        }

        FreeResources();
        GC.SuppressFinalize(other);
        _programId = other._programId;
        _uniforms = other._uniforms;
        _uniformBlocks = other._uniformBlocks;
    }
}