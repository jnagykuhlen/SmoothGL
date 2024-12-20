using SmoothGL.Content.Factories;
using SmoothGL.Content.Readers;
using SmoothGL.Graphics.Shader;
using SmoothGL.Graphics.Texturing;

namespace SmoothGL.Content;

/// <summary>
/// Provides extension methods for the <see cref="ContentManager"/> class.
/// </summary>
public static class ContentManagerExtension
{
    /// <summary>
    /// Registers content readers for all default content types.
    /// </summary>
    /// <param name="contentManager">The content manager readers are registered to.</param>
    /// <returns>The content manager readers are registered to.</returns>
    public static ContentManager SetDefaultContentReaders(this ContentManager contentManager) =>
        contentManager
            .SetContentReader(new SerializationReader())
            .SetContentReader(new Readers.StringReader())
            .SetContentReader(new ImageDataReader())
            .SetContentReader(new Texture2DReader(TextureFilterMode.Default))
            .SetContentReader(new TextureCubeReader(TextureFilterMode.Default))
            .SetContentReader(new FactoryReader<ShaderProgram, ShaderProgramFactory>())
            .SetContentReader(new WavefrontObjReader())
            .SetContentReader(new VertexArrayReader());
}