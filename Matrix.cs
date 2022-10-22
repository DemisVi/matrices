using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Matrix;

public static class Matrix
{
    public static float[,] ThreadedMultiply(float[,] matrixA, float[,] matrixB, int threadCount, bool isTransposed = false)
    {
        if (matrixA.Length <= 0 || matrixB.Length <= 0)
            throw new ArithmeticException("Matrix contains no elements!");

        var isExpectedSegments = TryGetSegments(out var segs, matrixA, threadCount);
        var segmrntRows = segs[0].GetLength(0);
        var segmentColumns = isTransposed ? matrixB.GetLength(0) : matrixB.GetLength(1);

        threadCount = isExpectedSegments ? threadCount : segs.Count;

        var result = new List<float[,]>(threadCount);

        using var countdownEvent = new CountdownEvent(threadCount);

        foreach (var seg in segs.Select((value, index) => new { value, index }))
        {
            result.Add(new float[segmrntRows, segmentColumns]);

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
        var targetHeight = source.Sum(x => x.GetLength(0));
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
        if (matrix.Length <= 0)
            throw new ArithmeticException("Matrix contains no elements!");

        var sourceRows = matrix.GetLength(0);
        var sourceColumns = matrix.GetLength(1);

        var segmentCount = sourceRows <= count ? sourceRows : count;
        var segmentRows = sourceRows > count ? sourceRows / count : 1;
        var remainder = sourceRows > count ? sourceRows % count : 0;

        var segmentSizesMap = Enumerable.Repeat(segmentRows, segmentCount).Select(x => 0 < remainder-- ? ++x : x).ToArray();
        var segmentSizeIndex = 0;

        dest = new List<float[,]>(segmentCount);

        for (int i = 0; i < sourceRows; i += segmentSizesMap[segmentSizeIndex++])
        {
            var temp = new float[segmentSizesMap[segmentSizeIndex], sourceColumns];

            for (int j = 0; j < segmentSizesMap[segmentSizeIndex]; j++)
                for (int k = 0; k < sourceColumns; k++)
                    temp[j, k] = matrix[i + j, k];
            dest.Add(temp);
        }

        return dest.Count == count;
    }
}