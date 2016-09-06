using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;


namespace SmoothGL.Graphics
{
    /// <summary>
    /// Represents the projection transformation required for stereoscopic rendering.
    /// Provides projection matrices for the left and the right eye, which allow to draw
    /// the scene twice to obtain stereo images.
    /// </summary>
    public class StereoProjection
    {
        private Matrix4 _leftProjection;
        private Matrix4 _rightProjection;

        /// <summary>
        /// Creates a new stereoscopic projection.
        /// </summary>
        /// <param name="monoProjection">Base projection matrix, which is the same as for a non-stereoscopic camera.</param>
        /// <param name="interaxial">Distance between the eyes.</param>
        /// <param name="screenWidth">Width of the virtual screen at convergence.</param>
        /// <param name="convergence">Distance between camera and virtual screen plane, where both view frustums converge.</param>
        public StereoProjection(Matrix4 monoProjection, float interaxial, float screenWidth, float convergence)
            : this(monoProjection, interaxial / screenWidth, convergence) { }

        /// <summary>
        /// Creates a new stereoscopic projection.
        /// </summary>
        /// <param name="monoProjection">Base projection matrix, which is the same as for a non-stereoscopic camera.</param>
        /// <param name="separation">Ratio of the distance between the eyes and the virtual screen width.</param>
        /// <param name="convergence">Distance between camera and virtual screen plane, where both view frustums converge.</param>
        public StereoProjection(Matrix4 monoProjection, float separation, float convergence)
        {
            _leftProjection = monoProjection;
            _leftProjection.M31 = separation;
            _leftProjection.M41 = separation * convergence;
            _rightProjection = monoProjection;
            _rightProjection.M31 = -separation;
            _rightProjection.M41 = -separation * convergence;
        }

        /// <summary>
        /// Gets the projection matrix for the left eye in the stereoscopic projection.
        /// </summary>
        public Matrix4 LeftProjection
        {
            get
            {
                return _leftProjection;
            }
        }

        /// <summary>
        /// Gets the projection matrix for the right eye in the stereoscopic projection.
        /// </summary>
        public Matrix4 RightProjection
        {
            get
            {
                return _rightProjection;
            }
        }
    }
}
