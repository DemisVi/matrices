using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Matrix;

public static class Matrix
{
    public static int[,] ThreadedMultiply(int[,] matrixA, int[,] matrixB, bool isTransposed = false)
    {
        var segCount = matrixA.GetLength(0) < Environment.ProcessorCount ? matrixA.GetLength(0) : Environment.ProcessorCount;

        var res = new List<int[,]>(segCount);

        using var countdownEvent = new CountdownEvent(segCount);

        foreach (var seg in GetSegments(matrixA, segCount).Select((value, index) => new { value, index }))
        {
            if (isTransposed)
                res.Add(new int[seg.value.GetLength(0), seg.value.GetLength(0)]);
            else
                res.Add(new int[seg.value.GetLength(0), seg.value.GetLength(1)]);

            ThreadPool.QueueUserWorkItem((_) =>
            {
                res[seg.index] = Multiply(seg.value, matrixB, isTransposed);
                countdownEvent.Signal();
            });
        }

        countdownEvent.Wait();

        return Group(res);
    }

    public static int[,] Multiply(int[,] matrixA, int[,] matrixB, bool isTransposed = false)
    {
        if (isTransposed) return MultiplyTransposed(matrixA, matrixB);

        if (matrixA.GetLength(1) != matrixB.GetLength(0))
            throw new ArithmeticException("Multiplication not possible. Is matrix B transposed?");

        var resultRowLength = matrixA.GetLength(0);
        var resultColumnLength = matrixB.GetLength(1);
        var initialBColumnLength = matrixB.GetLength(0);

        var result = new int[resultRowLength, resultColumnLength];

        var accumulator = 0;

        for (int i = 0; i < resultRowLength; i++)
            for (int j = 0; j < resultColumnLength; j++)
            {
                accumulator = 0;
                for (int k = 0; k < initialBColumnLength; k++)
                    accumulator += matrixA[i, k] * matrixB[k, j];
                result[i, j] = accumulator;
            }

        return result;
    }

    public static int[,] MultiplyTransposed(int[,] matrixA, int[,] matrixB)
    {
        if (matrixA.GetLength(1) != matrixB.GetLength(1))
            throw new ArithmeticException("Multiplication not possible. Is matrix B transposed?");

        var resultRowLength = matrixA.GetLength(0);
        var resultColumnLength = matrixB.GetLength(0);
        var initialBColumnLength = matrixB.GetLength(1);

        var result = new int[resultRowLength, resultColumnLength];

        var accumulator = 0;

        for (int i = 0; i < resultRowLength; i++)
            for (int j = 0; j < resultColumnLength; j++)
            {
                accumulator = 0;
                for (int k = 0; k < initialBColumnLength; k++)
                    accumulator += matrixA[i, k] * matrixB[j, k];
                result[i, j] = accumulator;
            }

        return result;
    }

    private static int[,] Group(List<int[,]> source)
    {
        var targetHeight = (source.Count - 1) * source.ElementAt(0).GetLength(0) + (source.Last().GetLength(0));
        var targetWidth = source.ElementAt(0).GetLength(1);

        var result = new int[targetHeight, targetWidth];

        var targetRowIndex = 0;
        foreach (var sourceSegment in source)
            for (var sourceRowIndex = 0; sourceRowIndex < sourceSegment.GetLength(0); sourceRowIndex++)
            {
                for (var columnIndex = 0; columnIndex < sourceSegment.GetLength(1); columnIndex++)
                    result[targetRowIndex, columnIndex] = sourceSegment[sourceRowIndex, columnIndex];
                targetRowIndex++;
            }

        return result;
    }

    private static List<int[,]> GetSegments(int[,] matrix, int count)
    {
        var matrixRows = matrix.GetLength(0);
        var matrixColumns = matrix.GetLength(1);
        var segmentRows = matrixRows % count != 0 ? matrixRows / count + 1 : matrixRows / count;
        var remainder = matrixRows % segmentRows;
        var result = new List<int[,]>(matrixRows > count ? count : matrixRows);

        for (var i = 0; i < matrixRows; i += segmentRows)
        {
            if (i + remainder > matrixRows - 1) segmentRows = remainder;
            var temp = new int[segmentRows, matrixColumns];
            for (var j = 0; j < matrixColumns; j++)
                for (var k = 0; k < segmentRows; k++)
                {
                    temp[k, j] = matrix[i + k, j];
                }
            result.Add(temp);
        }

        return result;
    }
}