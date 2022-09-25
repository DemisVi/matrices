using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

const int sizeA = 5;
const int sizeB = 2;

var matrixA = FillRandom(sizeA, sizeB, 10, 100);
var matrixB = FillRandom(sizeB, sizeA, 10, 100);

Print(matrixA);
Print(matrixB);

var result = Multiply(matrixA, matrixB);

Print(result);

var segmented = GetSegments(result, 3);

var targetHeight = (segmented.Count - 1) * segmented.ElementAt(0).GetLength(0) + (segmented.Last().GetLength(0));
var targetWidth = segmented.ElementAt(0).GetLength(1);

var res = new int[targetHeight, targetWidth];

var last = segmented[2];
var first = segmented[0];
Print(last);
Print(first);

for (var k = 0; k < segmented.Count; k++)
    for (var i = 0; i < segmented.ElementAt(k).GetLength(0); i++)
        for (var j = 0; j < segmented.ElementAt(k).GetLength(1); j++)
            res[i * (k + 1), j] = segmented[k][i, j];

Print(res);

System.Console.WriteLine(1);

int[,] Multiply(int[,] matrixa, int[,] matrixb)
{
    if (matrixa.GetLength(1) != matrixb.GetLength(0))
        throw new ArithmeticException("Multiplication not possible");

    var resultRowLength = matrixa.GetLength(0);
    var resultColumnLength = matrixb.GetLength(1);
    var initialBColumnLength = matrixb.GetLength(0);

    var result = new int[resultRowLength, resultColumnLength];

    var temp = 0;

    for (int i = 0; i < resultRowLength; i++)
        for (int j = 0; j < resultColumnLength; j++)
        {
            temp = 0;
            for (int k = 0; k < initialBColumnLength; k++)
                temp += matrixa[i, k] * matrixb[k, j];
            result[i, j] = temp;
        }

    return result;
}

void Print(int[,] matrix)
{
    for (var i = 0; i < matrix.GetLength(0); i++)
    {
        for (int j = 0; j < matrix.GetLength(1); j++)
            System.Console.Write("{0,-10}", matrix[i, j]);
        System.Console.WriteLine();
    }
    System.Console.WriteLine();
}

/* void Print(List<int[,]> matrix)
{
    foreach (var item in matrix)
        for (var i = 0; i < item.GetLength(0); i++)
        {
            for (int j = 0; j < item.GetLength(1); j++)
                System.Console.Write("{0,-10}", item[i, j]);
            System.Console.WriteLine();
        }
    System.Console.WriteLine();
} */

int[,] FillRandom(int rows, int cols, int min, int max)
{
    var rand = new Random(DateTime.Now.Millisecond);

    var temp = new int[rows, cols];

    for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            temp[i, j] = rand.Next(min, max);

    return temp;
}

List<int[,]> GetSegments(int[,] matrix, int count)
{
    var matrixRows = matrix.GetLength(0);
    var matrixColumns = matrix.GetLength(1);
    var segmentRows = matrixRows % count != 0 ? matrixRows / count + 1 : matrixRows / count;
    var remainder = matrixRows % segmentRows;
    var result = new List<int[,]>(count);

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