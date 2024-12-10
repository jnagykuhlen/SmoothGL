using System.Runtime.InteropServices;

namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderUniformArrayAssignment(IShaderUniformAssignment itemAssignment, int size) : IShaderUniformAssignment
{
    public bool Validate(object value) =>
        value is Array array && array.Length == size && array.OfType<object>().All(itemAssignment.Validate);

    public void Assign(int location, object value)
    {
        foreach (var item in (Array)value)
            itemAssignment.Assign(location++, item);
    }

    public void WriteToBuffer(IUnsafeBuffer buffer, object value, int offset)
    {
        foreach (var item in (Array)value)
        {
            itemAssignment.WriteToBuffer(buffer, item, offset);
            offset += Marshal.SizeOf(item);
        }
    }

    public bool IsPersistent => itemAssignment.IsPersistent;
}