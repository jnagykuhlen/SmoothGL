using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using SmoothGL.Graphics;


namespace SmoothGL.Content
{
    /// <summary>
    /// Serialializable, mutable factory which creates shader programs.
    /// </summary>
    [XmlRoot(ElementName = "ShaderProgram")]
    public class ShaderProgramFactory : IFactory<ShaderProgram>
    {
        /// <summary>
        /// Path to the vertex shader code file.
        /// </summary>
        [XmlElement(ElementName = "Vertex")]
        public string VertexShaderFilename { get; set; }

        /// <summary>
        /// Path to the tessellation control shader code file.
        /// Set this property to null if the tessellation control shader is not required.
        /// </summary>
        [XmlElement(ElementName = "TessellationControl")]
        public string TessellationControlFilename { get; set; }

        /// <summary>
        /// Path to the tessellation evaluation shader code file.
        /// Set this property to null if the tessellation evaluation shader is not required.
        /// </summary>
        [XmlElement(ElementName = "TessellationEvaluation")]
        public string TessellationEvaluationFilename { get; set; }

        /// <summary>
        /// Path to the geometry shader code file.
        /// Set this property to null if the geometry shader is not required.
        /// </summary>
        [XmlElement(ElementName = "Geometry")]
        public string GeometryShaderFilename { get; set; }

        /// <summary>
        /// Path to the fragment shader code file.
        /// </summary>
        [XmlElement(ElementName = "Fragment")]
        public string FragmentShaderFilename { get; set; }

        /// <summary>
        /// Creates a shader program from the individual shaders loaded from the corresponding files.
        /// </summary>
        /// <param name="contentManager">Content manager used to load the shader files.</param>
        /// <returns>Shader program.</returns>
        public ShaderProgram Create(ContentManager contentManager)
        {
            string vertexShaderCode = null;
            string tessellationControlShaderCode = null;
            string tessellationEvaluationShaderCode = null;
            string geometryShaderCode = null;
            string fragmentShaderCode = null;

            if (VertexShaderFilename != null)
                vertexShaderCode = LoadShaderCode(contentManager, VertexShaderFilename);
            if (TessellationControlFilename != null)
                tessellationControlShaderCode = LoadShaderCode(contentManager, TessellationControlFilename);
            if (TessellationEvaluationFilename != null)
                tessellationEvaluationShaderCode = LoadShaderCode(contentManager, TessellationEvaluationFilename);
            if (GeometryShaderFilename != null)
                geometryShaderCode = LoadShaderCode(contentManager, GeometryShaderFilename);
            if (FragmentShaderFilename != null)
                fragmentShaderCode = LoadShaderCode(contentManager, FragmentShaderFilename);

            try
            {
                return new ShaderProgram(
                    vertexShaderCode,
                    tessellationControlShaderCode,
                    tessellationEvaluationShaderCode,
                    geometryShaderCode,
                    fragmentShaderCode
                );
            }
            catch (ShaderCompilationException sce)
            {
                string sourceFile = "";
                switch (sce.ShaderType)
                {
                    case OpenTK.Graphics.OpenGL.ShaderType.VertexShader:
                        sourceFile = VertexShaderFilename;
                        break;
                    case OpenTK.Graphics.OpenGL.ShaderType.TessControlShader:
                        sourceFile = TessellationControlFilename;
                        break;
                    case OpenTK.Graphics.OpenGL.ShaderType.TessEvaluationShader:
                        sourceFile = TessellationEvaluationFilename;
                        break;
                    case OpenTK.Graphics.OpenGL.ShaderType.GeometryShader:
                        sourceFile = GeometryShaderFilename;
                        break;
                    case OpenTK.Graphics.OpenGL.ShaderType.FragmentShader:
                        sourceFile = FragmentShaderFilename;
                        break;
                }
                throw new ShaderCompilationException(string.Format("In File {0}: {1}", sourceFile, sce.Message), sce.ShaderType, sce.ShaderCode);
            }
        }

        private static string LoadShaderCode(ContentManager contentManager, string filename)
        {
            return LoadShaderCode(contentManager, UnifyPath(filename), new HashSet<string>());
        }

        private static string LoadShaderCode(ContentManager contentManager, string filename, HashSet<string> filesIncluded)
        {
            const string IncludeToken = "#include";

            string shaderCode = contentManager.Load<string>(filename);

            filesIncluded.Add(filename);

            int index;
            while ((index = shaderCode.IndexOf(IncludeToken)) >= 0)
            {
                int endIndex = shaderCode.IndexOf(Environment.NewLine, index);
                if (endIndex < 0)
                    endIndex = shaderCode.IndexOfAny(new char[] { '\n', '\r' }, index);
                if (endIndex < 0)
                    endIndex = shaderCode.Length;

                string includeCode = "";

                int pathIndex = index + IncludeToken.Length;
                string includePath = shaderCode.Substring(pathIndex, endIndex - pathIndex).Trim();

                if (includePath[0] == '\"' && includePath[includePath.Length - 1] == '\"')
                {
                    includePath = includePath.Substring(1, includePath.Length - 2);
                    includePath = UnifyPath(Path.Combine(Path.GetDirectoryName(filename), includePath));

                    if (!filesIncluded.Contains(includePath))
                    {
                        includeCode = LoadShaderCode(contentManager, includePath, filesIncluded);
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

        private static string UnifyPath(string path)
        {
            return path.ToLowerInvariant().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}
