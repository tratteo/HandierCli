// Copyright Matteo Beltrame

using System.Text;

namespace HandierCli;

public class Command : IEquatable<Command>
{
    private readonly List<Func<ArgumentsHandler, Task>> asyncActions;
    private readonly List<Action<ArgumentsHandler>> syncActions;
    private ArgumentsHandler? argsHandler;
    private TextWriter writer;
    private bool printHelp;
    private string helpFlag;
    private Func<Command, string>? printCallback;

    public string Description { get; private set; }

    public string Key { get; private set; }

    private Command(string key)
    {
        Key = key;
        asyncActions = new List<Func<ArgumentsHandler, Task>>();
        syncActions = new List<Action<ArgumentsHandler>>();
        argsHandler = null;
        writer = System.Console.Out;
        printHelp = true;
        Description = string.Empty;
        helpFlag = "-h";
    }

    public static Builder Factory(string key) => new Builder(key);

    public async Task Execute(string[] args)
    {
        if (argsHandler != null)
        {
            argsHandler.LoadArgs(args);
            if (argsHandler.HasFlag(helpFlag))
            {
                writer.WriteLine(Print());
            }
            else
            {
                if (argsHandler.Valid())
                {
                    await ExecuteParallel();
                }
                else if (printHelp)
                {
                    writer.WriteLine(Print());
                }
            }
        }
        else
        {
            await ExecuteParallel();
        }
    }

    public string Print()
    {
        if (printCallback != null)
        {
            return printCallback(this);
        }
        else
        {
            StringBuilder builder = new(Key);
            builder.Append('\t').Append(Description);
            if (argsHandler != null) builder.Append("\n" + argsHandler.Print());
            return builder.ToString();
        }
    }

    public bool Equals(Command? other) => Key.Equals(other?.Key);

    private async Task ExecuteParallel()
    {
        List<Task> tasks = new();
        foreach (var action in syncActions)
        {
            if (argsHandler != null)
            {
                action?.Invoke(argsHandler);
            }
        }
        foreach (var del in asyncActions)
        {
            if (del != null && argsHandler != null)
            {
                tasks.Add(del(argsHandler));
            }
        }
        await Task.WhenAll(tasks);
    }

    public class Builder
    {
        private readonly Command subroutine;
        private ArgumentsHandler.Builder argBuilder;

        public Builder(string key)
        {
            subroutine = new Command(key);
            argBuilder = new ArgumentsHandler.Builder();
        }

        public static implicit operator Command(Builder builder) => builder.Build();

        public Command Build()
        {
            if (subroutine.printHelp)
            {
                argBuilder.Flag(subroutine.helpFlag, "help");
            }
            subroutine.argsHandler = argBuilder.Build();
            return subroutine;
        }

        public Builder ArgumentsHandler(ArgumentsHandler.Builder handler)
        {
            argBuilder = handler;
            return this;
        }

        public Builder Out(TextWriter writer)
        {
            subroutine.writer = writer;
            return this;
        }

        public Builder Print(Func<Command, string> callback)
        {
            subroutine.printCallback = callback;
            return this;
        }

        public Builder AddAsync(Func<ArgumentsHandler, Task> task)
        {
            subroutine.asyncActions.Add(task);
            return this;
        }

        public Builder Add(Action<ArgumentsHandler> task)
        {
            subroutine.syncActions.Add(task);
            return this;
        }

        public Builder Description(string description)
        {
            subroutine.Description = description;
            return this;
        }

        public Builder InhibitHelp()
        {
            subroutine.printHelp = false;
            return this;
        }

        public Builder HelpFlag(string flag)
        {
            subroutine.helpFlag = flag;
            return this;
        }
    }
}