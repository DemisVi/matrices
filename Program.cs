using System;
using System.Text.Json;
using System.Numerics;

namespace Matrix;

public static class Prog
{
    public static void Main()
    {
        var m1 = new float[4, 4];

        FillRandom(ref m1);

        var m4 = new Matrix4x4(m1.GetValueBySingleIndex(0),
                               m1.GetValueBySingleIndex(1),
                               m1.GetValueBySingleIndex(2),
                               m1.GetValueBySingleIndex(3),
                               m1.GetValueBySingleIndex(4),
                               m1.GetValueBySingleIndex(5),
                               m1.GetValueBySingleIndex(6),
                               m1.GetValueBySingleIndex(7),
                               m1.GetValueBySingleIndex(8),
                               m1.GetValueBySingleIndex(9),
                               m1.GetValueBySingleIndex(10),
                               m1.GetValueBySingleIndex(11),
                               m1.GetValueBySingleIndex(12),
                               m1.GetValueBySingleIndex(13),
                               m1.GetValueBySingleIndex(14),
                               m1.GetValueBySingleIndex(15));



        System.Console.WriteLine(m1.AsString());
        System.Console.WriteLine(m4);

        System.Console.WriteLine(Matrix.Multiply(m1, m1, 4).AsString());
        System.Console.WriteLine(m4 * m4);
    }

    public static void FillRandom(ref float[,] result)
    {
        var rand = new Random(DateTime.Now.Millisecond);

        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = rand.NextSingle();
    }
}