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
    static readonly Dictionary<Angle, Angle> RotateMap = new()
    {
        [Angle.D0] = Angle.D90,
        [Angle.D90] = Angle.D180,
        [Angle.D180] = Angle.D270,
        [Angle.D270] = Angle.D0,
    };
    static readonly bool[,] _field = new bool[10, 30];
    static readonly Random _rng = new();
    static int _score;
    static long _figureTime;
    static long _currentTime;
    static bool _gameOver;
    static int _figureX;
    static int _figureY;
    static Figure _figure;
    static Angle _angle;
    static Move _move;
    static long _moveTime;
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
    enum Move
    {
        None,
        Left,
        Right,
        Rotate,
        Down
    }
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        NewFigure();
        while (!_gameOver)
        {
            ProcessInput();
            ProcessLogic();
            DrawField();
        }
        Console.WriteLine("game over");
        Console.ReadLine();
    }

    private static void NewFigure()
    {
        var x = _field.GetLength(0) / 2;
        var y = 2;
        if (CanSetFigure(x, y, _angle))
        {
            _figureX = x;
            _figureY = y;
        }
        else
        {
            _gameOver = true;
        }
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
            case ConsoleKey.LeftArrow:
                _move = Move.Left;
                break;

            case ConsoleKey.RightArrow:
                _move = Move.Right;
                break;

            case ConsoleKey.UpArrow:
                _move = Move.Rotate;
                break;

            case ConsoleKey.DownArrow:
                _move = Move.Down;
                break;
        }
    }

    static void ProcessLogic()
    {
        _currentTime = GetCurrentTime();
        var moveDelay = 50;
        var moveFigure = _currentTime - _moveTime > moveDelay;
        if (moveFigure)
        {
            _moveTime = _currentTime;
            var x = _figureX;
            var y = _figureY;
            switch (_move)
            {
                case Move.None:
                    break;

                case Move.Left:
                    --x;
                    break;

                case Move.Right:
                    ++x;
                    break;

                case Move.Down:
                    ++y;
                    break;

                case Move.Rotate:
                    _angle = RotateMap[_angle];
                    break;
            }

            if (x != _figureX || y != _figureY)
            {
                SetFigure(false);
                if (CanSetFigure(x, y, _angle))
                {
                    _figureX = x;
                    _figureY = y;
                }
                SetFigure(true);
            }

            _move = Move.None;
        }

        var restart = false;
        var descendDelay = 200;
        var descendFigure = _currentTime - _figureTime > descendDelay;
        if (descendFigure)
        {
            _figureTime = _currentTime;
            var newFigureY = _figureY + 1;
            SetFigure(false);
            if (CanSetFigure(_figureX, newFigureY, _angle))
            {
                _figureY = newFigureY;
            }
            else
            {
                restart = true;
            }
            SetFigure(true);
        }
        if (restart)
        {
            NewFigure();
        }
    }
    static void SetFigure(bool cell)
    {
        var key = (_figure, _angle);
        var coordinates = FigureMap[key];
        for (int i = 0; i < coordinates.Length; i++)
        {
            SetFigureCell(coordinates[i].Item1, coordinates[i].Item2, cell);
        }
    }
    static void SetFigureCell(int x, int y, bool cell)
    {
        _field[_figureX + x, _figureY + y] = cell;
    }
    static bool CanSetFigure(int figureX, int figureY, Angle angle)
    {
        var key = (_figure, angle);
        var coordinates = FigureMap[key];
        for (int i = 0; i < coordinates.Length; i++)
        {
            if (!CanSetFigureCell(coordinates[i].Item1 + figureX, coordinates[i].Item2 + figureY))
            {
                return false;
            }
        }
        return true;
    }

    private static bool CanSetFigureCell(int x, int y)
    {
        if (x < 0 || x >= _field.GetLength(0) || y < 0 || y >= _field.GetLength(1))
        {
            return false;
        }

        return !_field[x, y];

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
