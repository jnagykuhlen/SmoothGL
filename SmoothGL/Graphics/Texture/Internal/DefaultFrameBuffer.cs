using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics.Internal
{
    public class DefaultFrameBuffer : FrameBufferTarget
    {
        public DefaultFrameBuffer()
            : base(GetDefaultViewport()) { }

        private static Rectangle GetDefaultViewport()
        {
            int[] viewportParameters = new int[4];
            GL.GetInteger(GetPName.Viewport, viewportParameters);
            return new Rectangle(
                viewportParameters[0],
                viewportParameters[1],
                viewportParameters[2],
                viewportParameters[3]
            );
        }

        protected override int Id
        {
            get
            {
                return 0;
            }
        }

        protected override string ResourceName
        {
            get
            {
                return "DefaultFrameBuffer";
            }
        }
    }
}