using System;
using Matrix;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Matrix;

public class SimpleBench
{
    private float[,] m1 = new float[10, 10];
    private float[,] m2 = new float[10, 10];
    private float[,] m3 = new float[10, 10];
    private float[,] m4 = new float[10, 10];
    public SimpleBench()
    {
        FillRandom(ref m1);
        FillRandom(ref m2);
        FillRandom(ref m3);
        FillRandom(ref m4);
    }

    [Benchmark]
    public float[,] SingleThread() => Matrix.Multiply(m1, m2);

    [Benchmark]
    public float[,] SingleThreadTransposed() => Matrix.Multiply(m2, m3, true);

    [Benchmark]
    public float[,] MultiThread() => Matrix.ThreadedMultiply(m3, m4);

    [Benchmark]
    public float[,] MultiThreadTransposed() => Matrix.ThreadedMultiply(m3, m4, true);

    private void FillRandom(ref float[,] result)
    {
        var rand = new Random(DateTime.Now.Millisecond);

        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = rand.NextSingle();
    }
}

public static class Prog
{
    public static void Main()
    {
        var rand = new Random(DateTime.Now.Millisecond);

        for (int i = 1; i < 500; i++)
        {
            var m = new float[i, i];

            FillRandom(ref m);

            if (false == Matrix.TryGetSegments(out var res, m, 8))
            {
                System.Console.WriteLine("{0}: {1}, Segs: {2}, Rows: {3} ", i, 0, res.Count, m.GetLength(0));

                foreach (var j in res)
                    System.Console.Write("{0,5}", j.GetLength(0));

                System.Console.WriteLine();
            }
        }


        // SimpleBench sb = new();
        // System.Console.WriteLine(sb.MultiThread().AsString());
        // System.Console.WriteLine(sb.MultiThreadTransposed().AsString());
        // var some = BenchmarkRunner.Run<SimpleBench>();

    }

    public static void FillRandom(ref float[,] result)
    {
        var rand = new Random(DateTime.Now.Millisecond);

        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = rand.NextSingle();
    }
}