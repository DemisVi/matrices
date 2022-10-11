using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Matrix;

public static class Matrix
{
    public static float[,] ThreadedMultiply(float[,] matrixA, float[,] matrixB, bool isTransposed = false)
    {
        
        var result = new List<float[,]>(segCount);
        var resultColumnHeight = matrixA.GetLength(0);
        var resultRowLength = isTransposed ? matrixB.GetLength(0) : matrixB.GetLength(1);

        var segCount = matrixA.GetLength(0) < Environment.ProcessorCount ? matrixA.GetLength(0) : Environment.ProcessorCount;
        using var countdownEvent = new CountdownEvent(segCount);

        foreach (var seg in .Select((value, index) => new { value, index }))
        {
            result.Add(new float[resultColumnHeight, resultRowLength]);

            ThreadPool.QueueUserWorkItem((_) =>
            {
                result[seg.index] = Multiply(seg.value, matrixB, isTransposed);
                countdownEvent.Signal();
            });
        }

        countdownEvent.Wait();

        return Group(result);
    }

    public static float[,] Transpose(float[,] source)
    {
        var result = new float[source.GetLength(1), source.GetLength(0)];

        for (var i = 0; i < source.GetLength(0); i++)
            for (var j = 0; j < source.GetLength(1); j++)
                result[j, i] = source[i, j];

        return result;
    }

    public static float[,] Multiply(float[,] matrixA, float[,] matrixB, bool isTransposed = false)
    {
        if (isTransposed) return MultiplyTransposed(matrixA, matrixB);

        if (matrixA.GetLength(1) != matrixB.GetLength(0))
            throw new ArithmeticException("Multiplication not possible. Is matrix B transposed?");

        var resultRowLength = matrixA.GetLength(0);
        var resultColumnHeight = matrixB.GetLength(1);
        var initialBColumnHeight = matrixB.GetLength(0);

        var result = new float[resultRowLength, resultColumnHeight];

        var accumulator = 0f;

        for (int i = 0; i < resultRowLength; i++)
            for (int j = 0; j < resultColumnHeight; j++)
            {
                accumulator = 0;
                for (int k = 0; k < initialBColumnHeight; k++)
                    accumulator += matrixA[i, k] * matrixB[k, j];
                result[i, j] = accumulator;
            }

        return result;
    }

    public static float[,] MultiplyTransposed(float[,] matrixA, float[,] matrixB)
    {
        if (matrixA.GetLength(1) != matrixB.GetLength(1))
            throw new ArithmeticException("Multiplication not possible. Is matrix B transposed?");

        var resultRowLength = matrixA.GetLength(0);
        var resultColumnHeight = matrixB.GetLength(0);
        var initialBColumnLength = matrixB.GetLength(1);

        var result = new float[resultRowLength, resultColumnHeight];

        var accumulator = 0f;

        for (int i = 0; i < resultRowLength; i++)
            for (int j = 0; j < resultColumnHeight; j++)
            {
                accumulator = 0;
                for (int k = 0; k < initialBColumnLength; k++)
                    accumulator += matrixA[i, k] * matrixB[j, k];
                result[i, j] = accumulator;
            }

        return result;
    }

    private static float[,] Group(List<float[,]> source)
    {
        var targetHeight = (source.Count - 1) * source.ElementAt(0).GetLength(0) + (source.Last().GetLength(0));
        var targetWidth = source.ElementAt(0).GetLength(1);

        var result = new float[targetHeight, targetWidth];

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

    public static bool TryGetSegments(out List<float[,]> dest, float[,] matrix, int count)
    {
        var matrixRows = matrix.GetLength(0);
        var matrixColumns = matrix.GetLength(1);
        var segmentRows = matrixRows % count != 0 ? matrixRows / count + 1 : matrixRows / count;
        var remainder = matrixRows % segmentRows;
        dest = new List<float[,]>(matrixRows > count ? count : matrixRows);

        for (var i = 0; i < matrixRows; i += segmentRows)
        {
            if (i + remainder > matrixRows - 1) segmentRows = remainder;
            var temp = new float[segmentRows, matrixColumns];
            for (var j = 0; j < matrixColumns; j++)
                for (var k = 0; k < segmentRows; k++)
                    temp[k, j] = matrix[i + k, j];
            dest.Add(temp);
        }

        return dest.Count == count;
    }
}