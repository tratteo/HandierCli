// Copyright Matteo Beltrame

using Microsoft.Extensions.Logging;

namespace HandierCli.Log;

/// <summary>
///   A <see cref="Console"/> logger that implements <see cref="ILogger"/> and has some more functionalities such as custom color and <see cref="LogLevel"/>
/// </summary>
public class AdvancedLogger : ILogger
{
    public bool Enabled { get; set; } = true;

    public AdvancedLogger()
    {
    }

    public void Log(LogLevel level, string log, bool newLine = true) => Log(log, GetColorByLogLevel(level), newLine);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) => Log(logLevel, formatter(state, exception), true);

    public bool IsEnabled(LogLevel logLevel) => Enabled;

    public IDisposable BeginScope<TState>(TState state) => default!;

    public virtual void Log(string log, ConsoleColor color, bool newLine = true)
    {
        var currentColor = System.Console.ForegroundColor;
        Console.ForegroundColor = color;
        if (newLine)
        {
            Console.WriteLine(log);
        }
        else
        {
            Console.Write(log);
        }
        Console.ForegroundColor = currentColor;
        Console.Out.Flush();
    }

    protected virtual ConsoleColor GetColorByLogLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => ConsoleColor.DarkGray,
            LogLevel.Debug => ConsoleColor.DarkMagenta,
            LogLevel.Information => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.DarkYellow,
            LogLevel.Error => ConsoleColor.DarkRed,
            LogLevel.Critical => ConsoleColor.Magenta,
            LogLevel.None => ConsoleColor.White,
            _ => ConsoleColor.White,
        };
    }
}