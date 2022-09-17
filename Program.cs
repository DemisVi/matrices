using System;
using System.Numerics;

int sizeA = 4;
int sizeB = 3;

var matrixA = Fill(sizeA, sizeB);
var matrixB = Fill(sizeB, sizeA);

Print(matrixA);
Print(matrixB);

Print(Multiply(matrixA, matrixB));

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

int[,] Fill(int rows, int cols)
{
    var rand = new Random(DateTime.Now.Millisecond);

    var temp = new int[rows, cols];

    for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            temp[i, j] = rand.Next(0, 101);

    return temp;
}
