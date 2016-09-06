using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SmoothGL.Graphics.Internal;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents a shader program linked from a number of compiled shaders.
    /// </summary>
    public class ShaderProgram : GraphicsResource
    {
        private static int _currentProgramId = 0;

        private int _programId;
        private Dictionary<string, ShaderProgramUniform> _uniforms;
        private Dictionary<string, ShaderUniformBlock> _uniformBlocks;

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
        public ShaderProgram(string vertexShaderCode,
                             string tessellationControlShaderCode,
                             string tessellationEvaluationShaderCode,
                             string geometryShaderCode, 
                             string fragmentShaderCode)
        {
            List<int> shaderIds = new List<int>(5);

            try
            {
                if (vertexShaderCode != null)
                    shaderIds.Add(CreateShader(ShaderType.VertexShader, vertexShaderCode));
                if (tessellationControlShaderCode != null)
                    shaderIds.Add(CreateShader(ShaderType.TessControlShader, tessellationControlShaderCode));
                if (tessellationEvaluationShaderCode != null)
                    shaderIds.Add(CreateShader(ShaderType.TessEvaluationShader, tessellationEvaluationShaderCode));
                if (geometryShaderCode != null)
                    shaderIds.Add(CreateShader(ShaderType.GeometryShader, geometryShaderCode));
                if (fragmentShaderCode != null)
                    shaderIds.Add(CreateShader(ShaderType.FragmentShader, fragmentShaderCode));

                _programId = LinkProgram(shaderIds.ToArray());
            }
            finally
            {
                foreach (int shaderId in shaderIds)
                    DeleteShader(shaderId);
            }

            InitializeUniforms();
        }

        private static int LinkProgram(int[] shaderIds)
        {
            int programId = GL.CreateProgram();
            foreach (int shaderId in shaderIds)
                GL.AttachShader(programId, shaderId);
            
            GL.LinkProgram(programId);

            int linked;
            GL.GetProgram(programId, GetProgramParameterName.LinkStatus, out linked);

            if (linked == 0)
            {
                string message = GL.GetProgramInfoLog(programId);
                GL.DeleteProgram(programId);
                throw new ShaderProgramLinkException(message);
            }

            return programId;
        }

        private int CreateShader(ShaderType shaderType, string shaderCode)
        {
            int shaderId = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderId, shaderCode);
            GL.CompileShader(shaderId);
            
            int compiled;
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out compiled);

            if (compiled == 0)
            {
                string message = GL.GetShaderInfoLog(shaderId);
                GL.DeleteShader(shaderId);
                throw new ShaderCompilationException(message, shaderType, shaderCode);
            }

            return shaderId;
        }

        private void DeleteShader(int shaderId)
        {
            GL.DeleteShader(shaderId);
        }

        private void InitializeUniforms()
        {
            GL.UseProgram(_programId);

            int numberOfUniforms;
            int numberOfUniformBlocks;

            GL.GetProgram(_programId, GetProgramParameterName.ActiveUniforms, out numberOfUniforms);
            GL.GetProgram(_programId, GetProgramParameterName.ActiveUniformBlocks, out numberOfUniformBlocks);

            _uniforms = new Dictionary<string, ShaderProgramUniform>(numberOfUniforms, StringComparer.Ordinal);
            _uniformBlocks = new Dictionary<string, ShaderUniformBlock>(numberOfUniformBlocks, StringComparer.Ordinal);

            List<UniformBufferElement>[] uniformBlockElements = new List<UniformBufferElement>[numberOfUniformBlocks];
            for (int i = 0; i < numberOfUniformBlocks; ++i)
                uniformBlockElements[i] = new List<UniformBufferElement>();
            
            int textureIndex = 0;
            int maxTextureIndex = GL.GetInteger(GetPName.MaxCombinedTextureImageUnits);

            for (int i = 0; i < numberOfUniforms; ++i)
            {
                int uniformSize;
                int uniformBlockIndex;
                ActiveUniformType uniformRawType;
                string uniformName = GL.GetActiveUniform(_programId, i, out uniformSize, out uniformRawType);
                int uniformLocation = GL.GetUniformLocation(_programId, uniformName);
                GL.GetActiveUniforms(_programId, 1, ref i, ActiveUniformParameter.UniformBlockIndex, out uniformBlockIndex);
                ShaderUniformType uniformType = (ShaderUniformType)uniformRawType;

                if (uniformName.EndsWith("[0]"))
                    uniformName = uniformName.Substring(0, uniformName.Length - 3);

                if (uniformBlockIndex == -1)
                {
                    if (!Enum.IsDefined(typeof(ShaderUniformType), uniformType))
                    {
                        throw new ShaderUniformException(
                            String.Format("The uniform type {0} specified in the shader for uniform {1} is not supported.", uniformType, uniformName),
                            uniformName,
                            uniformType
                        );
                    }
                    
                    IShaderUniformAssignmentDispatcher dispatcher = ShaderUniformSingleAssignmentDispatcher.Instance;
                    if (uniformSize > 1)
                        dispatcher = ShaderUniformArrayAssignmentDispatcher.Instance;

                    if (uniformType == ShaderUniformType.Sampler1D ||
                        uniformType == ShaderUniformType.Sampler2D ||
                        uniformType == ShaderUniformType.Sampler3D ||
                        uniformType == ShaderUniformType.SamplerCube)
                    {
                        if (textureIndex + uniformSize > maxTextureIndex)
                        {
                            throw new ShaderUniformException(
                                String.Format("Texture uniform {0} exceeds the limit of {1} texture units.", uniformName, maxTextureIndex),
                                uniformName,
                                uniformType
                            );
                        }

                        int[] indices = new int[uniformSize];
                        for (int j = 0; j < uniformSize; ++j)
                            indices[j] = textureIndex + j;

                        GL.Uniform1(uniformLocation, uniformSize, indices);
                        
                        uniformLocation = textureIndex;
                        textureIndex += uniformSize;
                    }
                    
                    ShaderProgramUniform uniform = new ShaderProgramUniform(uniformName, uniformType, uniformSize, uniformLocation, dispatcher);
                    _uniforms.Add(uniformName, uniform);
                }
                else
                {
                    // Currently, arrays in uniform blocks are not supported.
                    if (uniformSize == 1)
                    {
                        int uniformOffset;
                        GL.GetActiveUniforms(_programId, 1, ref i, ActiveUniformParameter.UniformOffset, out uniformOffset);
                        uniformBlockElements[uniformBlockIndex].Add(new UniformBufferElement(uniformName, uniformType, uniformOffset));
                    }
                }
            }
            
            for (int i = 0; i < numberOfUniformBlocks; ++i)
            {
                int uniformBlockSize;
                string uniformBlockName = GL.GetActiveUniformBlockName(_programId, i);
                GL.GetActiveUniformBlock(_programId, i, ActiveUniformBlockParameter.UniformBlockDataSize, out uniformBlockSize);
                GL.UniformBlockBinding(_programId, i, i);

                UniformBufferLayout layout = new UniformBufferLayout(uniformBlockSize, uniformBlockElements[i].ToArray());
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

            if (_currentProgramId != _programId)
            {
                GL.UseProgram(_programId);
                _currentProgramId = _programId;
            }

            foreach (ShaderProgramUniform uniform in _uniforms.Values)
                uniform.Apply();
            
            foreach (ShaderUniformBlock uniformBlock in UniformBlocks)
            {
                if (uniformBlock.Buffer != null)
                    uniformBlock.Buffer.Bind(uniformBlock.Location);
            }
        }

        /// <summary>
        /// Gets the uniform with the specified name. Returns null if such uniform does not exist.
        /// Note that uniform value changes are not communicated to the GPU until this shader
        /// program's <see cref="Use"/> method is called again.
        /// </summary>
        /// <param name="name">Name of the uniform.</param>
        /// <returns>Uniform.</returns>
        public ShaderUniform Uniform(string name)
        {
            ShaderProgramUniform result;
            if (_uniforms.TryGetValue(name, out result))
                return result;
            return null;
        }

        /// <summary>
        /// Gets the uniform block with the specified name. Returns null if such uniform block does not exist.
        /// </summary>
        /// <param name="name">Name of the uniform block.</param>
        /// <returns>Uniform block.</returns>
        public ShaderUniformBlock UniformBlock(string name)
        {
            ShaderUniformBlock result;
            if (_uniformBlocks.TryGetValue(name, out result))
                return result;
            return null;
        }

        /// <summary>
        /// Gets all uniforms defined by this shader program.
        /// </summary>
        public IEnumerable<ShaderUniform> Uniforms
        {
            get
            {
                return _uniforms.Values;
            }
        }

        /// <summary>
        /// Gets all uniform blocks defined by this shader program.
        /// </summary>
        public IEnumerable<ShaderUniformBlock> UniformBlocks
        {
            get
            {
                return _uniformBlocks.Values;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this shader program is currently in use for subsequent draw operations,
        /// i.e., its <see cref="Use"/> method was called after using any other shader program.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _currentProgramId == _programId;
            }
        }

        protected override void FreeResources()
        {
            if (_currentProgramId == _programId)
                _currentProgramId = 0;

            if(_programId != 0)
                GL.DeleteProgram(_programId);
        }

        protected override string ResourceName
        {
            get
            {
                return "ShaderProgram";
            }
        }
    }
}
