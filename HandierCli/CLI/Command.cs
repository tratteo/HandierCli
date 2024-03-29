﻿// Copyright Matteo Beltrame

using HandierCli.Log;
using System.Text;

namespace HandierCli.CLI;

public delegate string CommandPrintDelegate(Command command);

public delegate Task AsyncCommandActionDelegate(ArgumentsHandler argumentsHandler);

public delegate void SyncCommandActionDelegate(ArgumentsHandler argumentsHandler);

public partial class Command : IEquatable<Command>
{
    private readonly List<AsyncCommandActionDelegate> asyncActions;
    private readonly List<SyncCommandActionDelegate> syncActions;
    private ArgumentsHandler? argsHandler;
    private AdvancedLogger logger;
    private string helpFlag;
    private CommandPrintDelegate? printCallback;

    private ConsoleColor? consoleColor;

    public string Description { get; private set; }

    public string Key { get; private set; }

    private Command(string key)
    {
        Key = key;
        consoleColor = null;
        asyncActions = new List<AsyncCommandActionDelegate>();
        syncActions = new List<SyncCommandActionDelegate>();
        argsHandler = null;
        logger = new AdvancedLogger();
        Description = string.Empty;
        helpFlag = "-h";
    }

    public static Builder Factory(string key) => new Builder(key);

    /// <summary>
    ///   Execute the command with the provided arguments
    /// </summary>
    /// <returns> </returns>
    public async Task Execute(IEnumerable<string> args)
    {
        if (argsHandler != null)
        {
            argsHandler.LoadArgs(args);
            if (argsHandler.HasFlag(helpFlag))
            {
                WriteLine(Print());
            }
            else
            {
                var res = argsHandler.Fits();
                if (res.Successful)
                {
                    await ExecuteParallel();
                }
                else
                {
                    WriteLine(res.Reason);
                    foreach (var f in res.FailedFits)
                    {
                        WriteLine(f.Item2 + " is not valid for argument " + f.Item1);
                    }
                    WriteLine("\nUsage: ");
                    WriteLine(Print());
                }
            }
        }
        else
        {
            await ExecuteParallel();
        }
    }

    /// <summary>
    ///   Print the command and its <see cref="ArgumentsHandler"/>
    /// </summary>
    /// <returns> </returns>
    public string Print()
    {
        if (printCallback != null)
        {
            return printCallback(this);
        }
        else
        {
            StringBuilder builder = new("█ ");
            builder.Append(Key);
            builder.Append('\t').Append(Description);
            if (argsHandler != null) builder.Append("\n" + argsHandler.Print());
            return builder.ToString();
        }
    }

    /// <inheritdoc/>
    public bool Equals(Command? other) => Key.Equals(other?.Key);

    private void WriteLine(string line)
    {
        var c = consoleColor ?? Console.ForegroundColor;
        logger.Log(line, c);
    }

    private async Task ExecuteParallel()
    {
        List<Task> tasks = new();
        foreach (var del in asyncActions)
        {
            if (del != null && argsHandler != null)
            {
                tasks.Add(del(argsHandler));
            }
        }
        foreach (var action in syncActions)
        {
            if (argsHandler != null)
            {
                action?.Invoke(argsHandler);
            }
        }

        await Task.WhenAll(tasks);
    }
}