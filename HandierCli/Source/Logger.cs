// Copyright Matteo Beltrame

using System;

namespace HandierCli;

public class Logger
{
    private static Logger INSTANCE = new Logger();

    public static Logger ConsoleInstance
    {
        get
        {
            if (INSTANCE == null)
            {
                INSTANCE = new Logger();
            }
            return INSTANCE;
        }
    }

    public bool Enabled { get; set; } = true;

    public Logger()
    {
    }

    public void LogInfo(string log, bool newLine = true) => LogInfo(log, ConsoleColor.White, newLine);

    public virtual void LogInfo(string log, ConsoleColor color, bool newLine = true)
    {
        ConsoleColor currentColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = color;
        if (newLine)
        {
            System.Console.WriteLine(log);
        }
        else
        {
            System.Console.Write(log);
        }
        System.Console.ForegroundColor = currentColor;
        System.Console.Out.Flush();
    }

    public virtual void LogWarning(string log, bool newLine = true) => LogInfo("W: " + log, ConsoleColor.DarkYellow, newLine);

    public virtual void LogError(string log, bool newLine = true) => LogInfo("E: " + log, ConsoleColor.Red, newLine);
}