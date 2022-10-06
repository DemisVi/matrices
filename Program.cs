using System;
using Matrix;

namespace Matrix;

public static class SimpleBench
{
    public static void Main()
    {
        var m1 = new int[5, 5];
        var m2 = new int[5, 5];

        FillRandom(ref m1, 20, 30);
        FillRandom(ref m2, 20, 30);

        var start = DateTime.Now;

        var s1 = Matrix.ThreadedMultiply(m1, m2).AsString();

        System.Console.WriteLine(DateTime.Now - start);

        FillRandom(ref m1, 20, 30);
        FillRandom(ref m2, 20, 30);

        start = DateTime.Now;
        
        var s2 = Matrix.Multiply(m1, m2).AsString();

        System.Console.WriteLine(DateTime.Now - start);

        // void Print(List<int[,]> matrix)
        // {
        //     foreach (var item in matrix)
        //         for (var i = 0; i < item.GetLength(0); i++)
        //         {
        //             for (int j = 0; j < item.GetLength(1); j++)
        //                 System.Console.Write("{0,-10}", item[i, j]);
        //             System.Console.WriteLine();
        //         }
        //     System.Console.WriteLine();
        // }

        void FillRandom(ref int[,] result, int min, int max)
        {
            var rand = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = rand.Next(min, max);
        }
    }
}