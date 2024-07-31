namespace SmoothGL.Graphics.Internal;

public class ShaderUniformSingleAssignmentDispatcher : IShaderUniformAssignmentDispatcher
{
    public static readonly ShaderUniformSingleAssignmentDispatcher Instance = new();

    public void Assign(IShaderUniformAssignment assignment, int location, object value)
    {
        assignment.AssignSingle(location, value);
    }

    public void Validate(IShaderUniformAssignment assignment, ShaderUniform uniform, object value)
    {
        assignment.ValidateSingle(uniform, value);
    }
}