using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Texturing;

/// <summary>
/// Defines a texture persistent in graphics memory.
/// </summary>
public abstract class Texture : GraphicsResource
{
    private readonly TextureTarget _target;
    private int _textureId;

    protected Texture(TextureTarget target, TextureFilterMode filterMode)
    {
        _target = target;
        FilterMode = filterMode;
        GL.GenTextures(1, out _textureId);
        GL.BindTexture(_target, _textureId);
        ApplyFiltering();
    }

    /// <summary>
    /// Gets the filter mode of this texture.
    /// </summary>
    public TextureFilterMode FilterMode { get; }

    protected int Id => _textureId;

    protected override string ResourceName => "Texture";

    /// <summary>
    /// Binds this texture to the graphics device. This method is not required to be called by client code.
    /// </summary>
    public void Bind()
    {
        GL.BindTexture(_target, _textureId);
    }

    protected void UpdateMipmaps()
    {
        if (FilterMode.Mipmapping)
            GL.GenerateMipmap((GenerateMipmapTarget)_target);
    }

    private void ApplyFiltering()
    {
        TextureMinFilter minFilter;
        TextureMagFilter magFilter;

        if (FilterMode.Interpolation == TextureInterpolation.Nearest)
        {
            minFilter = FilterMode.Mipmapping ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest;
            magFilter = TextureMagFilter.Nearest;
        }
        else
        {
            minFilter = FilterMode.Mipmapping ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear;
            magFilter = TextureMagFilter.Linear;
        }

        GL.TexParameter(_target, TextureParameterName.TextureMinFilter, (int)minFilter);
        GL.TexParameter(_target, TextureParameterName.TextureMagFilter, (int)magFilter);

        GL.TexParameter(_target, TextureParameterName.TextureWrapS, (int)FilterMode.Wrap);
        GL.TexParameter(_target, TextureParameterName.TextureWrapT, (int)FilterMode.Wrap);
        GL.TexParameter(_target, TextureParameterName.TextureWrapR, (int)FilterMode.Wrap);

        GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out float maxAnisotropy);

        var anisotropy = MathHelper.Clamp(FilterMode.Anisotropy, 1.0f, maxAnisotropy);
        GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, anisotropy);
    }

    protected sealed override void FreeResources()
    {
        GL.DeleteTextures(1, ref _textureId);
    }
}