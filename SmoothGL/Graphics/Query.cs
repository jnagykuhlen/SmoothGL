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
    /// Abstract query used to perform GPU measurements.
    /// </summary>
    public abstract class Query : GraphicsResource
    {
        private QueryTarget _target;
        private int _queryId;
        private bool _active;

        /// <summary>
        /// Creates a new query.
        /// </summary>
        /// <param name="target">Specifies the type of the query.</param>
        protected Query(QueryTarget target)
        {
            _target = target;
            _active = false;
            GL.GenQueries(1, out _queryId);
        }

        /// <summary>
        /// Starts the query. An InvalidOperationException is thrown if the query is already active.
        /// </summary>
        public void Begin()
        {
            if (_active)
                throw new InvalidOperationException("Cannot begin query because it is already active.");
            GL.BeginQuery(_target, _queryId);
            _active = true;
        }

        /// <summary>
        /// Ends the query. An InvalidOperationException is thrown if the query is not active.
        /// Not that the query result may not be immediately available.
        /// </summary>
        public void End()
        {
            if (!_active)
                throw new InvalidOperationException("Cannot end query because it is not active.");
            GL.EndQuery(_target);
            _active = false;
        }

        /// <summary>
        /// Gets a value indicating whether the query has been performed successfully and the result is available.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                if (_active)
                    return false;

                int finished;
                GL.GetQueryObject(_queryId, GetQueryObjectParam.QueryResultAvailable, out finished);
                return finished != 0;
            }
        }

        /// <summary>
        /// Gets the query result if it is available.
        /// </summary>
        protected int Result
        {
            get
            {
                if (_active)
                    throw new InvalidOperationException("End query before accessing the result.");

                int result;
                GL.GetQueryObject(_queryId, GetQueryObjectParam.QueryResult, out result);
                return result;
            }
        }

        protected int Id
        {
            get
            {
                return _queryId;
            }
        }

        protected override void FreeResources()
        {
            GL.DeleteQueries(1, ref _queryId);
        }

        protected override string ResourceName
        {
            get
            {
                return "Query";
            }
        }
    }
}
