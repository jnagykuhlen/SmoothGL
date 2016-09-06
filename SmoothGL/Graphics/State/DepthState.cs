using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Encapsulates rendering state variables related to depth testing. Rendering state objects are immutable.
    /// </summary>
    public class DepthState : IGraphicsState
    {
        /// <summary>
        /// Defines classical depth testing, discarding fragments with a depth value greater than the value
        /// in the depth buffer. When a fragment passes the depth test, the depth buffer is updated accordingly.
        /// </summary>
        public static readonly DepthState Default = new DepthState(true, true, CompareFunction.LessEqual);

        /// <summary>
        /// Defines classical depth testing, discarding fragments with a depth value greater than the value
        /// in the depth buffer. However, the depth buffer is not updated when a fragment passed the depth test.
        /// </summary>
        public static readonly DepthState DepthRead = new DepthState(true, false, CompareFunction.LessEqual);

        /// <summary>
        /// Defines a depth state without depth testing, updating the depth buffer in any case.
        /// </summary>
        public static readonly DepthState DepthWrite = new DepthState(true, true, CompareFunction.Always);
        
        /// <summary>
        /// Defines a state which disables depth testing and writing entirely.
        /// </summary>
        public static readonly DepthState None = new DepthState(false, false, CompareFunction.LessEqual);

        /// <summary>
        /// Gets a value indicating whether a depth test is performed for fragments, discarding the fragment
        /// if the depth comparison fails.
        /// </summary>
        public bool DepthTestEnabled { get; private set; }
        
        /// <summary>
        /// Gets a value indicating whether the depth buffer is updated when a fragment passes the depth test.
        /// </summary>
        public bool DepthWriteEnabled { get; private set; }

        /// <summary>
        /// Gets the depth compare function which needs to evaluate to true for fragment to pass the depth test.
        /// </summary>
        public CompareFunction DepthFunction { get; private set; }

        /// <summary>
        /// Creates a new depth state object.
        /// </summary>
        /// <param name="depthTestEnabled">Defines whether a depth test is performed for fragments.</param>
        /// <param name="depthWriteEnabled">Defines whether the depth buffer is updated when a fragment passes the depth test.</param>
        /// <param name="depthFunction">Defines the depth compare function which needs to evaluate true for fragments to pass the depth test.</param>
        public DepthState(bool depthTestEnabled, bool depthWriteEnabled, CompareFunction depthFunction)
        {
            DepthTestEnabled = depthTestEnabled;
            DepthWriteEnabled = depthWriteEnabled;
            DepthFunction = depthFunction;
        }

        /// <summary>
        /// Communicates the state encapsulated in this state object to the driver.
        /// </summary>
        public void Apply()
        {
            if (DepthTestEnabled)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc((DepthFunction)DepthFunction);
            }
            else
            {
                GL.Disable(EnableCap.DepthTest);
            }

            GL.DepthMask(DepthWriteEnabled);
        }
    }
}
