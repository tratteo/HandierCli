// Copyright Matteo Beltrame

namespace HandierCli;

public static class ConsoleExtensions
{
    public static void ClearConsoleLine(int inverseIndex = 0)
    {
        var currentLineCursor = System.Console.CursorTop;
        System.Console.SetCursorPosition(0, System.Console.CursorTop - inverseIndex);
        System.Console.Write(new string(' ', System.Console.WindowWidth));
        System.Console.SetCursorPosition(0, currentLineCursor);
    }
}