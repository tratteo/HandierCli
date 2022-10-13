// Copyright Matteo Beltrame

using HandierCli.Log;
using HandierCli.Statics;

namespace HandierCli.CLI;

public partial class CommandLine
{
    /// <summary>
    ///   Class inheriting from <see cref="AdvancedLogger"/> that automatically takes care of managing the <see cref="CommandLine"/> symbol
    ///   when logging
    /// </summary>
    public class Logger : AdvancedLogger
    {
        private readonly CommandLine cli;

        public Logger(CommandLine cli)
        {
            this.cli = cli;
        }

        public override void Log(string log, ConsoleColor color, bool newLine = true)
        {
            if (!cli.IsExecutingCommand && cli.IsRunning)
            {
                ConsoleExtensions.ClearConsoleLine();
                base.Log(log, color, newLine);
                if (!newLine) Console.WriteLine();
                Console.Write(cli.symbol);
                Console.Out.Flush();
            }
            else
            {
                base.Log(log, color, newLine);
            }
        }
    }
}