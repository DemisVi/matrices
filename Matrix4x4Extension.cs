using System;
using System.Numerics;

public static class Matrix4x4Extension
{
    public static float[,] ToSingle(this Matrix4x4 source)
    {
        return new[,]
        {
            { source.M11, source.M12, source.M13, source.M14, },
            { source.M21, source.M22, source.M23, source.M24, },
            { source.M31, source.M32, source.M33, source.M34, },
            { source.M41, source.M42, source.M43, source.M44 }
        };
    }
}
