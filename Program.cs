using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
static class Program
{
    static readonly Dictionary<(Figure, Angle), (int, int)[]> FigureMap = new()
    {
        [(Figure.L, Angle.D0)] = new[] { (0, 0), (0, -1), (0, 1), (1, 1) },
        [(Figure.L, Angle.D90)] = new[] { (0, 0), (-1, 0), (1, 0), (1, -1) },
        [(Figure.L, Angle.D180)] = new[] { (0, 0), (0, 1), (0, -1), (-1, -1) },
        [(Figure.L, Angle.D270)] = new[] { (0, 0), (-1, 0), (1, 0), (-1, 1) },
        [(Figure.O, Angle.D0)] = new[] { (0, 0), (0, 1), (1, 1), (1, 0) },
        [(Figure.O, Angle.D90)] = new[] { (0, 0), (0, 1), (1, 1), (1, 0) },
        [(Figure.O, Angle.D180)] = new[] { (0, 0), (0, 1), (1, 1), (1, 0) },
        [(Figure.O, Angle.D270)] = new[] { (0, 0), (0, 1), (1, 1), (1, 0) },
        [(Figure.I, Angle.D0)] = new[] { (0, -2), (0, -1), (0, 0), (0, 1) },
        [(Figure.I, Angle.D90)] = new[] { (-2, 0), (-1, 0), (0, 0), (1, 0) },
        [(Figure.I, Angle.D180)] = new[] { (0, -2), (0, -1), (0, 0), (0, 1)},
        [(Figure.I, Angle.D270)] = new[] {(-2, 0), (-1, 0), (0, 0), (1, 0) },
        	
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
    enum Figure : byte
    {
        L,
        O,
        I,
    }
    enum Angle : short
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
        _figure = (Figure)_rng.Next(3);
        var x = _field.GetLength(0) / 2;
        var y = 2;
        var angle = (Angle)_rng.Next(4);
        if (CanSetFigure(x, y, angle))
        {
            _figureX = x;
            _figureY = y;
            _angle = angle;
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

            case ConsoleKey.S:
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
            var angle = _angle;
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
                    angle = RotateMap[_angle];
                    break;

            }

            if (x != _figureX || y != _figureY || angle != _angle)
            {
                SetFigure(false);
                if (CanSetFigure(x, y, angle))
                {
                    _figureX = x;
                    _figureY = y;
                    _angle = angle;
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

        for (int row = 0; row < _field.GetLength(0); row++)
        {
            var b = true;
            for (int column = 0; column < _field.GetLength(1); column++)
            {
                if (!_field[row, column])
                {
                    b = false;
                }
            }
            if (b)
            {
                for (int column = 0; column < _field.GetLength(1); column++)
                {
                    _field[row, column] = false;
                }
            }
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
        DrawField('#', '.');
    }
    static void DrawField(char figure, char empty)
    {
        Console.SetCursorPosition(0, 0);
        for (int row = 0; row < _field.GetLength(1); row++)
        {
            for (int column = 0; column < _field.GetLength(0); column++)
            {
                if (_field[column, row])
                {
                    Console.Write(figure);
                }
                else
                {
                    Console.Write(empty);
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine($"{Math.Max(_score, 0):D4}");
    }
    static void DrawField(ConsoleColor color, char figure, char empty)
    {
        Console.ForegroundColor = color;
        DrawField(figure, empty);
        Console.ForegroundColor = ConsoleColor.White;
    }
}
/*๐	သုည1	θòʊɴɲa̰	туньа
1	၁	တစ်	tɪʔ	ти
2	၂	နှစ်	n̥ɪʔ	хни
3	၃	သုံး	θóʊɴ	тун
4	၄	လေး	lé	ле
5	၅	ငါး	ŋá	нга
6	၆	ာက်	tɕʰaʊʔ	чхау
7	၇	ခုနစ်	kʰʊ̀ɴ n̥ɪʔ2	кхуни
8	၈	ရှစ်	ʃɪʔ	ши
9	၉	ကိုး	kó	ко
10	၁၀	ဆယ်	sʰɛ̀	схэ*/