using OpenTK.Graphics.OpenGL;

namespace SmoothGL.Graphics;

/// <summary>
/// Abstract query used to perform GPU measurements.
/// </summary>
public abstract class Query : GraphicsResource
{
    private bool _active;
    private int _queryId;
    private readonly QueryTarget _target;

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
    /// Gets a value indicating whether the query has been performed successfully and the result is available.
    /// </summary>
    public bool IsFinished
    {
        get
        {
            if (_active)
                return false;

            GL.GetQueryObject(_queryId, GetQueryObjectParam.QueryResultAvailable, out int finished);
            return finished != 0;
        }
    }

    /// <summary>
    /// Gets the query result if it is available.
    /// </summary>
    protected long Result
    {
        get
        {
            if (_active)
                throw new InvalidOperationException("End query before accessing the result.");

            GL.GetQueryObject(_queryId, GetQueryObjectParam.QueryResult, out long result);
            return result;
        }
    }

    protected int Id => _queryId;

    protected override string ResourceName => "Query";

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

    protected override void FreeResources()
    {
        GL.DeleteQueries(1, ref _queryId);
    }
}