// Copyright Matteo Beltrame

using System.Text;

namespace HandierCli.CLI;

public partial class CommandLine
{
    private readonly List<Command> commands;

    private readonly List<string> exitCommands;

    private Action<Logger, string>? onUnrecognized;

    private string symbol;

    private bool canReceiveCommands;
    private string? globalHelpSymbol;
    private ConsoleColor helpColor;

    public Logger CliLogger { get; private set; }

    public bool IsExecutingCommand { get; private set; } = false;

    public bool IsRunning { get; private set; } = false;

    private CommandLine()
    {
        CliLogger = new Logger(this);
        helpColor = ConsoleColor.DarkGray;
        canReceiveCommands = true;
        globalHelpSymbol = null;
        commands = new List<Command>();
        exitCommands = new List<string>();
        symbol = "> ";
    }

    public static Builder Factory() => new();

    /// <summary>
    ///   Register a new <see cref="Command.Builder"/>
    /// </summary>
    /// <param name="command"> </param>
    /// <returns> </returns>
    public CommandLine Register(Command.Builder command)
    {
        if (!string.IsNullOrWhiteSpace(globalHelpSymbol))
        {
            command.HelpFlag(globalHelpSymbol);
        }
        command.HelpLogger(CliLogger, helpColor);
        if (!commands.Contains(command))
        {
            commands.Add(command);
        }
        return this;
    }

    public void CanReceiveCommands(bool state)
    {
        canReceiveCommands = state;
        if (canReceiveCommands)
        {
            Console.Out.Flush();
            Console.WriteLine();
            Console.Write(symbol);
        }
    }

    /// <summary>
    ///   Run the CLI asynchronously
    /// </summary>
    /// <returns> </returns>
    public async Task RunAsync()
    {
        IsRunning = true;
        var command = string.Empty;
        while (!ShouldExit(command))
        {
            if (!canReceiveCommands) continue;
            if (canReceiveCommands)
            {
                Console.Write(symbol);
            }
            command = Console.ReadLine();
            command ??= string.Empty;
            command = command.Trim();
            if (ShouldExit(command)) return;
            var argsList = ParseString(command);
            if (argsList != null && argsList.Length > 0)
            {
                var routine = commands.Find(s => s.Key.Equals(argsList[0]));
                if (routine == null)
                {
                    IsExecutingCommand = true;
                    onUnrecognized?.Invoke(CliLogger, argsList[0]);
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

    /// <summary>
    ///   Print the CLI, displays all the registerd commands and their usage
    /// </summary>
    /// <returns> </returns>
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
}