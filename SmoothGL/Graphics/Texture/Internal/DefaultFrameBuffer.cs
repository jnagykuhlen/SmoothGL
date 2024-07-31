using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics.Internal;

public class DefaultFrameBuffer : FrameBufferTarget
{
    public DefaultFrameBuffer()
        : base(GetDefaultViewport())
    {
    }

    protected override int Id => 0;

    protected override string ResourceName => "DefaultFrameBuffer";

    private static Rectangle GetDefaultViewport()
    {
        var viewportParameters = new int[4];
        GL.GetInteger(GetPName.Viewport, viewportParameters);
        return new Rectangle(
            viewportParameters[0],
            viewportParameters[1],
            viewportParameters[2],
            viewportParameters[3]
        );
    }
}