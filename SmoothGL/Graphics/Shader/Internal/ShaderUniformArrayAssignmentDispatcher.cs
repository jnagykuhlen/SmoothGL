namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderUniformArrayAssignmentDispatcher : IShaderUniformAssignmentDispatcher
{
    public static readonly ShaderUniformArrayAssignmentDispatcher Instance = new();

    public void Assign(IShaderUniformAssignment assignment, int location, object value)
    {
        assignment.AssignArray(location, value);
    }

    public void Validate(IShaderUniformAssignment assignment, ShaderUniform uniform, object value)
    {
        assignment.ValidateArray(uniform, value);
    }
}