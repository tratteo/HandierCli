// Copyright Matteo Beltrame

namespace HandierCli.Statics;

public static class ConsoleExtensions
{
    /// <summary>
    ///   Clear the specified <see cref="Console"/> cline
    /// </summary>
    /// <param name="inverseIndex">
    ///   Indicates the line to delete. Examples: -1 means the second to last row, -4 the fourth to last row
    /// </param>
    public static void ClearConsoleLine(int inverseIndex = 0)
    {
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop - inverseIndex);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
}