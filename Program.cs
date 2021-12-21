using System;
using System.Collections.Generic;
using System.Diagnostics;

static class Program
{
    static readonly Dictionary<(Figure, Angle), (int, int)[]> FigureMap = new()
    {
        [(Figure.L, Angle.D0)] = new[] { (0, 0), (0, -1), (0, 1), (1, 1) },
        [(Figure.L, Angle.D90)] = new[] { (0, 0), (-1, 0), (1, 0), (1, -1) },
        [(Figure.L, Angle.D180)] = new[] { (0, 0), (0, 1), (0, -1), (-1, -1) },
        [(Figure.L, Angle.D270)] = new[] { (0, 0), (-1, 0), (1, 0), (-1, 1) },
    };
    static readonly bool[,] _field = new bool[10, 30];
    static readonly Random _rng = new();
    static int _score;
    static long _currentTime;
    static bool _gameOver;
    static int _figureX;
    static int _figureY;
    static Figure _figure;
    static Angle _angle;
    enum Figure
    {
        L,
    }
    enum Angle
    {
        D0,
        D90,
        D180,
        D270,
    }
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        _figureX = _field.GetLength(0) / 2;
        _figureY = 2;
        while (!_gameOver)
        {
            ProcessInput();
            ProcessLogic();
            DrawField();
        }
        Console.ReadLine();
    }

    static void ProcessInput()
    {
        if (!Console.KeyAvailable)
        {
            return;
        }

        var key = Console.ReadKey(true);
        switch (key.Key)
        {


        }
    }

    static void ProcessLogic()
    {
        _currentTime = GetCurrentTime();
        SetFigure(true);
    }
    static void SetFigure(bool cell)
    {
        var key = (_figure, _angle);
        var coordinates = FigureMap[key];
        for (int i = 0; i < coordinates.Length; i++)
        {
            SetFigureCell(coordinates[i].Item1, coordinates[i].Item2, true);
        }
    }
    static void SetFigureCell(int x, int y, bool cell)
    {
        _field[_figureX + x, _figureY + y] = cell;
    }
    static long GetCurrentTime()
    {
        return Stopwatch.GetTimestamp() * 1000 / Stopwatch.Frequency;
    }

    static void DrawField()
    {
        Console.SetCursorPosition(0, 0);
        for (int row = 0; row < _field.GetLength(1); row++)
        {
            for (int column = 0; column < _field.GetLength(0); column++)
            {
                if (_field[column, row])
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine($"{Math.Max(_score, 0):D4}");
    }
}
