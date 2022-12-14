using System;
using System.Text;

namespace Matrix;

public static class ArrayExtension
{
    public static string? AsString(this float[,]? source)
    {
        if (source is null) return null;

        var sb = new StringBuilder(source!.GetLength(0) * source!.GetLength(1));

        for (var i = 0; i < source.GetLength(0); i++)
        {
            sb.AppendFormat("{0:N6}", source[i, 0]);

            for (var j = 1; j < source.GetLength(1); j++)
                sb.AppendFormat(" {0:N6}", source[i, j]);

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }
    
    public static string? AsString(this float[][]? source)
    {
        if (source is null) return null;

        var sb = new StringBuilder(source!.GetLength(0) * source!.GetLength(1));

        for (var i = 0; i < source.GetLength(0); i++)
        {
            sb.AppendFormat("{0:N6}", source[i][0]);

            for (var j = 1; j < source.GetLength(1); j++)
                sb.AppendFormat(" {0:N6}", source[i][j]);

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }

    public static float GetValueBySingleIndex(this float[,] source, int index)
    {
        var row = Math.DivRem(index, source.GetLength(1), out int column);

        return source[row, column];
    }
}