// Copyright Matteo Beltrame

using Microsoft.Extensions.Logging;
using System.Text;

namespace HandierCli;

public class CommandLine
{
    private readonly List<Command> commands;

    private readonly List<string> exitCommands;

    private Action<CommandLineLogger, string>? onUnrecognized;

    private string symbol;

    public CommandLineLogger Logger { get; private set; }

    public bool IsExecutingCommand { get; private set; } = false;

    public bool IsRunning { get; private set; } = false;

    private CommandLine()
    {
        Logger = new CommandLineLogger(this);
        commands = new List<Command>();
        exitCommands = new List<string>();
        symbol = "> ";
    }

    public static Builder Factory() => new();

    public CommandLine Register(Command command)
    {
        if (!commands.Contains(command))
        {
            commands.Add(command);
        }
        return this;
    }

    public async Task RunAsync()
    {
        IsRunning = true;
        var command = string.Empty;
        while (!ShouldExit(command))
        {
            System.Console.Write(symbol);
            command = System.Console.ReadLine();
            command ??= string.Empty;
            command = command.Trim();
            if (ShouldExit(command))
            {
                return;
            }
            var argsList = ParseString(command);
            if (argsList != null && argsList.Length > 0)
            {
                var routine = commands.Find(s => s.Key.Equals(argsList[0]));
                if (routine == null)
                {
                    IsExecutingCommand = true;
                    onUnrecognized?.Invoke(Logger, argsList[0]);
                    IsExecutingCommand = false;
                }
                else
                {
                    IsExecutingCommand = true;
                    await routine.Execute(argsList.Skip(1).ToArray());
                    IsExecutingCommand = false;
                }
            }
        }
        IsRunning = false;
    }

    public string Print()
    {
        StringBuilder builder = new();

        for (var i = 0; i < commands.Count; i++)
        {
            builder.Append('\n');
            builder.Append(commands[i].Print());

            if (i < commands.Count - 1)
            {
                builder.Append('\n');
            }
        }
        return builder.ToString();
    }

    private static string[] ParseString(string str)
    {
        var retval = new List<string>();
        if (string.IsNullOrWhiteSpace(str)) return retval.ToArray();
        var ndx = 0;
        var s = string.Empty;
        var insideDoubleQuote = false;
        var insideSingleQuote = false;

        while (ndx < str.Length)
        {
            if (str[ndx] == ' ' && !insideDoubleQuote && !insideSingleQuote)
            {
                if (!string.IsNullOrWhiteSpace(s.Trim()))
                {
                    s = s.Replace("\"", string.Empty).Replace("\'", string.Empty);
                    retval.Add(s.Trim());
                }

                s = string.Empty;
            }
            if (str[ndx] == '"') insideDoubleQuote = !insideDoubleQuote;
            if (str[ndx] == '\'') insideSingleQuote = !insideSingleQuote;
            s += str[ndx];
            ndx++;
        }
        if (!string.IsNullOrWhiteSpace(s.Trim()))
        {
            s = s.Replace("\"", string.Empty).Replace("\'", string.Empty);
            retval.Add(s.Trim());
        }
        return retval.ToArray();
    }

    private bool ShouldExit(string command) => exitCommands.Contains(command);

    public class Builder
    {
        private readonly CommandLine commandLine;

        public Builder()
        {
            commandLine = new CommandLine();
        }

        public static implicit operator CommandLine(Builder builder) => builder.Build();

        public Builder ExitOn(params string[] exitCommands)
        {
            commandLine.exitCommands.AddRange(exitCommands);
            return this;
        }

        public Builder CliSymbol(string symbol)
        {
            commandLine.symbol = symbol;
            return this;
        }

        public Builder RegisterHelpCommand(Command? helpCommandOverride = null)
        {
            helpCommandOverride ??= Command.Factory("help")
                    .InhibitHelp()
                    .Description("display the available commands")
                    .ArgumentsHandler(ArgumentsHandler.Factory())
                    .Add((handler) => commandLine.Logger.Log(LogLevel.Information, commandLine.Print()));
            commandLine.Register(helpCommandOverride);
            return this;
        }

        public Builder OnUnrecognized(Action<CommandLineLogger, string> callback)
        {
            commandLine.onUnrecognized = callback;
            return this;
        }

        public CommandLine Build() => commandLine;
    }

    public class CommandLineLogger : BaseLogger
    {
        private readonly CommandLine cli;

        public CommandLineLogger(CommandLine cli)
        {
            this.cli = cli;
        }

        protected override void Log(string log, ConsoleColor color, bool newLine = true)
        {
            if (!cli.IsExecutingCommand && cli.IsRunning)
            {
                ConsoleExtensions.ClearConsoleLine();
                base.Log(log, color, newLine);
                if (!newLine) System.Console.WriteLine();
                System.Console.Write(cli.symbol);
                System.Console.Out.Flush();
            }
            else
            {
                base.Log(log, color, newLine);
            }
        }
    }
}