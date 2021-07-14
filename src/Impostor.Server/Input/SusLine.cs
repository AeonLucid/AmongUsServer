using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Impostor.Server.Input
{
    /// <summary>
    /// GNU Readline-like utility for collecting user input.
    /// </summary>
    internal class SusLine : IDisposable
    {
        private readonly StringBuilder _input = new StringBuilder();
        private readonly SusLineWriter _writer;

        private readonly List<string> _history = new List<string>();
        private int _historyPosition;

        public SusLine()
        {
            _writer = new SusLineWriter(Console.Out, _input);
            Console.SetOut(_writer);
        }

        /// <summary>
        /// Custom <see cref="Console.ReadLine"/> with support for being used alongside <see cref="Console.SetCursorPosition"/> calls and history support.
        /// </summary>
        /// <param name="stoppingToken"><see cref="CancellationToken"/> for a while loop.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public async ValueTask<string?> ReadLineAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Console.KeyAvailable)
                {
                    continue;
                }

                var keyInfo = await Task<ConsoleKeyInfo>.Factory.StartNew(static _ => Console.ReadKey(true), this, stoppingToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                    {
                        var line = _input.ToString();
                        Clear();

                        AppendToHistory(line);
                        return line;
                    }

                    case ConsoleKey.LeftArrow:
                        MoveCursorLeft();
                        break;

                    case ConsoleKey.RightArrow:
                        MoveCursorRight();
                        break;

                    case ConsoleKey.Home:
                        MoveCursorToBeginning();
                        break;

                    case ConsoleKey.End:
                        MoveCursorToEnd();
                        break;

                    case ConsoleKey.Backspace:
                        Backspace();
                        break;

                    case ConsoleKey.Delete:
                        Delete();
                        break;

                    case ConsoleKey.UpArrow:
                    {
                        if (_historyPosition > 0)
                        {
                            _historyPosition--;
                            SetInput(GetHistoryElement());
                        }

                        break;
                    }

                    case ConsoleKey.DownArrow:
                    {
                        if (_historyPosition < _history.Count)
                        {
                            _historyPosition++;
                            SetInput(GetHistoryElement());
                        }

                        break;
                    }

                    default:
                    {
                        if (keyInfo.Key == ConsoleKey.Clear || keyInfo.KeyChar == (char)ConsoleKey.Clear)
                        {
                            Console.Clear();
                            break;
                        }

                        if (keyInfo.KeyChar == default)
                        {
                            continue;
                        }

                        _input.Insert(_writer.Position, keyInfo.KeyChar);
                        _writer.Position++;
                        break;
                    }
                }

                Update();
            }

            Clear();
            return null;
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Update()
        {
            _writer.Update();
        }

        public void MoveCursorLeft()
        {
            if (_writer.Position > 0)
            {
                _writer.Position -= 1;
            }
        }

        public void MoveCursorRight()
        {
            if (_writer.Position < _input.Length)
            {
                _writer.Position += 1;
            }
        }

        public void MoveCursorToBeginning()
        {
            _writer.Position = 0;
        }

        public void MoveCursorToEnd()
        {
            _writer.Position = _input.Length;
        }

        public void Backspace()
        {
            if (_writer.Position > 0 && _writer.Position <= _input.Length)
            {
                _input.Remove(_writer.Position - 1, 1);
                MoveCursorLeft();
            }
        }

        public void Delete()
        {
            if (_input.Length > _writer.Position)
            {
                _input.Remove(_writer.Position, 1);
            }
        }

        public void AppendToHistory(string line)
        {
            _history.Add(line);
            _historyPosition++;
        }

        public void SetInput(string? line)
        {
            _input.Clear();

            if (line != null)
            {
                _input.Append(line);
                MoveCursorToEnd();
            }
            else
            {
                MoveCursorToBeginning();
            }
        }

        private string? GetHistoryElement()
        {
            return _historyPosition != _history.Count ? _history[_historyPosition] : null;
        }

        private void Clear()
        {
            _writer.Position = 0;
            _input.Clear();
        }

        /// <summary>
        /// Console.Out wrapper used for displaying prompts.
        /// </summary>
        private class SusLineWriter : TextWriter
        {
            private const string Prefix = "> ";

            private readonly TextWriter _out;
            private readonly StringBuilder _input;
            private readonly StringBuilder _buffer = new StringBuilder();

            private int _lastLength = -1;
            private int _lastPosition = -1;
            private ConsoleColor _lastColor = (ConsoleColor)(-1);

            public SusLineWriter(TextWriter @out, StringBuilder input)
            {
                _out = @out;
                _input = input;

                Encoding = _out.Encoding;
            }

            public override Encoding Encoding { get; }

            public int Position { get; set; }

            public override void Write(char value)
            {
                if (value == '\n')
                {
                    WriteLine();
                    return;
                }

                lock (Console.Out)
                {
                    try
                    {
                        if (_lastColor != Console.ForegroundColor)
                        {
                            _buffer.Append(Console.ForegroundColor switch
                            {
                                ConsoleColor.Black => Ansi.Color.Foreground.Black.EscapeSequence,
                                ConsoleColor.DarkBlue => Ansi.Color.Foreground.Blue.EscapeSequence,
                                ConsoleColor.DarkGreen => Ansi.Color.Foreground.Green.EscapeSequence,
                                ConsoleColor.DarkCyan => Ansi.Color.Foreground.Cyan.EscapeSequence,
                                ConsoleColor.DarkRed => Ansi.Color.Foreground.Red.EscapeSequence,
                                ConsoleColor.DarkMagenta => Ansi.Color.Foreground.Magenta.EscapeSequence,
                                ConsoleColor.DarkYellow => Ansi.Color.Foreground.Yellow.EscapeSequence,
                                ConsoleColor.Gray => Ansi.Color.Foreground.White.EscapeSequence,
                                ConsoleColor.DarkGray => Ansi.Color.Foreground.DarkGray.EscapeSequence,
                                ConsoleColor.Blue => Ansi.Color.Foreground.LightBlue.EscapeSequence,
                                ConsoleColor.Green => Ansi.Color.Foreground.LightGreen.EscapeSequence,
                                ConsoleColor.Cyan => Ansi.Color.Foreground.LightCyan.EscapeSequence,
                                ConsoleColor.Red => Ansi.Color.Foreground.LightRed.EscapeSequence,
                                ConsoleColor.Magenta => Ansi.Color.Foreground.LightMagenta.EscapeSequence,
                                ConsoleColor.Yellow => Ansi.Color.Foreground.LightYellow.EscapeSequence,
                                ConsoleColor.White => Ansi.Color.Foreground.LightGray.EscapeSequence,
                                _ => Ansi.Color.Foreground.Default.EscapeSequence,
                            });
                        }

                        _buffer.Append(value);
                        _lastColor = Console.ForegroundColor;
                    }
                    catch (Exception e)
                    {
                        _out.WriteLine(e);
                    }
                }
            }

            public override void WriteLine()
            {
                lock (Console.Out)
                {
                    try
                    {
                        ClearPrompt();

                        _out.WriteLine(_buffer);
                        _buffer.Clear();

                        WritePrompt();
                    }
                    catch (Exception e)
                    {
                        _out.WriteLine(e);
                    }
                }
            }

            public void Update()
            {
                lock (Console.Out)
                {
                    ClearPrompt();
                    WritePrompt();
                }
            }

            protected override void Dispose(bool disposing)
            {
                lock (Console.Out)
                {
                    ClearPrompt();
                    Console.SetOut(_out);
                }

                base.Dispose(disposing);
            }

            private static (int Lines, int Line) CalculateLines(int length, int position, int width)
            {
                return (length / width, position / width);
            }

            private void WritePrompt()
            {
                var input = Prefix + _input;

                if (Console.ForegroundColor != ConsoleColor.White)
                {
                    Console.ResetColor();
                }

                _out.Write(input);

                var width = Console.WindowWidth;
                var position = Prefix.Length + Position;
                var (lines, line) = CalculateLines(input.Length, position, width);

                Console.SetCursorPosition(position % width, Console.CursorTop + (line - lines));

                _lastPosition = position;
                _lastLength = input.Length;
            }

            private void ClearPrompt()
            {
                if (_lastLength == -1 || _lastPosition == -1)
                {
                    return;
                }

                var (left, top) = Console.GetCursorPosition();

                if (left != 0 || top != 0)
                {
                    var width = Console.WindowWidth;

                    Console.CursorVisible = false;

                    var (lines, line) = CalculateLines(_lastLength, _lastPosition, width);
                    var start = top - line;

                    if (_lastLength % width == 0)
                    {
                        start++;
                    }

                    for (var i = 0; i <= lines; i++)
                    {
                        ClearLine(start + i);
                    }

                    Console.SetCursorPosition(0, start);

                    Console.CursorVisible = true;
                }

                _lastLength = -1;
                _lastPosition = -1;
            }

            private void ClearLine(int line)
            {
                Console.SetCursorPosition(0, line);
                _out.Write(Ansi.Clear.Line.EscapeSequence);
            }
        }
    }
}
