using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SmoothGL.Graphics.Shader.Internal;

public class ShaderUniformAssignmentBool : ShaderUniformAssignment<bool>
{
    protected override void AssignSingle(int location, bool value)
    {
        GL.Uniform1(location, BoolToInt(value));
    }

    protected override void AssignArray(int location, bool[] value)
    {
        GL.Uniform1(location, value.Length, value.Select(BoolToInt).ToArray());
    }

    public override void WriteSingleToBuffer(IUnsafeBuffer buffer, object value, int offset)
    {
        buffer.SetData(BoolToInt((bool)value), offset);
    }

    private static int BoolToInt(bool value)
    {
        return value ? 1 : 0;
    }
}

public class ShaderUniformAssignmentInt : ShaderUniformAssignment<int>
{
    protected override void AssignSingle(int location, int value)
    {
        GL.Uniform1(location, value);
    }

    protected override void AssignArray(int location, int[] value)
    {
        GL.Uniform1(location, value.Length, value);
    }
}

public class ShaderUniformAssignmentUnsignedInt : ShaderUniformAssignment<uint>
{
    protected override void AssignSingle(int location, uint value)
    {
        GL.Uniform1(location, value);
    }

    protected override void AssignArray(int location, uint[] value)
    {
        GL.Uniform1(location, value.Length, value);
    }
}

public class ShaderUniformAssignmentFloat : ShaderUniformAssignment<float>
{
    protected override void AssignSingle(int location, float value)
    {
        GL.Uniform1(location, value);
    }

    protected override void AssignArray(int location, float[] value)
    {
        GL.Uniform1(location, value.Length, value);
    }
}

public class ShaderUniformAssignmentDouble : ShaderUniformAssignment<double>
{
    protected override void AssignSingle(int location, double value)
    {
        GL.Uniform1(location, value);
    }

    protected override void AssignArray(int location, double[] value)
    {
        GL.Uniform1(location, value.Length, value);
    }
}

public class ShaderUniformAssignmentVector2 : ShaderUniformAssignment<Vector2>
{
    protected override void AssignSingle(int location, Vector2 value)
    {
        GL.Uniform2(location, value);
    }

    protected override void AssignArray(int location, Vector2[] value)
    {
        unsafe
        {
            fixed (float* p = &value[0].X)
            {
                GL.Uniform2(0, 2 * value.Length, p);
            }
        }
    }
}

public class ShaderUniformAssignmentVector3 : ShaderUniformAssignment<Vector3>
{
    protected override void AssignSingle(int location, Vector3 value)
    {
        GL.Uniform3(location, value);
    }

    protected override void AssignArray(int location, Vector3[] value)
    {
        unsafe
        {
            fixed (float* p = &value[0].X)
            {
                GL.Uniform3(0, 3 * value.Length, p);
            }
        }
    }
}

public class ShaderUniformAssignmentVector4 : ShaderUniformAssignment<Vector4>
{
    protected override void AssignSingle(int location, Vector4 value)
    {
        GL.Uniform4(location, value);
    }

    protected override void AssignArray(int location, Vector4[] value)
    {
        unsafe
        {
            fixed (float* p = &value[0].X)
            {
                GL.Uniform4(0, 4 * value.Length, p);
            }
        }
    }
}

public class ShaderUniformAssignmentDoubleVector2 : ShaderUniformAssignment<Vector2d>
{
    protected override void AssignSingle(int location, Vector2d value)
    {
        GL.Uniform2(location, value.X, value.Y);
    }

    protected override void AssignArray(int location, Vector2d[] value)
    {
        unsafe
        {
            fixed (double* p = &value[0].X)
            {
                GL.Uniform2(0, 2 * value.Length, p);
            }
        }
    }
}

public class ShaderUniformAssignmentDoubleVector3 : ShaderUniformAssignment<Vector3d>
{
    protected override void AssignSingle(int location, Vector3d value)
    {
        GL.Uniform3(location, value.X, value.Y, value.Z);
    }

    protected override void AssignArray(int location, Vector3d[] value)
    {
        unsafe
        {
            fixed (double* p = &value[0].X)
            {
                GL.Uniform3(0, 3 * value.Length, p);
            }
        }
    }
}

public class ShaderUniformAssignmentDoubleVector4 : ShaderUniformAssignment<Vector4d>
{
    protected override void AssignSingle(int location, Vector4d value)
    {
        GL.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    protected override void AssignArray(int location, Vector4d[] value)
    {
        unsafe
        {
            fixed (double* p = &value[0].X)
            {
                GL.Uniform4(0, 4 * value.Length, p);
            }
        }
    }
}

public class ShaderUniformAssignmentMatrix2 : ShaderUniformAssignment<Matrix2>
{
    protected override void AssignSingle(int location, Matrix2 value)
    {
        GL.UniformMatrix2(location, false, ref value);
    }

    protected override void AssignArray(int location, Matrix2[] value)
    {
        unsafe
        {
            fixed (float* p = &value[0].Row0.X)
            {
                GL.UniformMatrix2(0, 4 * value.Length, false, p);
            }
        }
    }
}

public class ShaderUniformAssignmentMatrix3 : ShaderUniformAssignment<Matrix3>
{
    protected override void AssignSingle(int location, Matrix3 value)
    {
        GL.UniformMatrix3(location, false, ref value);
    }

    protected override void AssignArray(int location, Matrix3[] value)
    {
        unsafe
        {
            fixed (float* p = &value[0].Row0.X)
            {
                GL.UniformMatrix3(0, 9 * value.Length, false, p);
            }
        }
    }
}

public class ShaderUniformAssignmentMatrix4 : ShaderUniformAssignment<Matrix4>
{
    protected override void AssignSingle(int location, Matrix4 value)
    {
        GL.UniformMatrix4(location, false, ref value);
    }

    protected override void AssignArray(int location, Matrix4[] value)
    {
        unsafe
        {
            fixed (float* p = &value[0].Row0.X)
            {
                GL.UniformMatrix4(0, 16 * value.Length, false, p);
            }
        }
    }
}