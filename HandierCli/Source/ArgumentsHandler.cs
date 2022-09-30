// Copyright Matteo Beltrame

using System.Text;

namespace HandierCli;

public class ArgumentsHandler
{
    private readonly Dictionary<string, string> keyedArgs;

    private readonly List<string> flagArgs;

    private readonly List<string> positionalArgs;

    private Func<ArgumentsHandler, string>? printCallback;

    public List<Argument> Flags { get; private set; }

    public List<Argument> Positionals { get; private set; }

    public List<Argument> Keys { get; private set; }

    private ArgumentsHandler()
    {
        positionalArgs = new List<string>();
        keyedArgs = new Dictionary<string, string>();
        flagArgs = new List<string>();

        Keys = new List<Argument>();
        Flags = new List<Argument>();
        Positionals = new List<Argument>();
    }

    public static Builder Factory() => new Builder();

    public string GetPositional(int index) => positionalArgs[index];

    public bool GetKeyed(string key, out string value)
    {
        value = string.Empty;
        if (keyedArgs.ContainsKey(key))
        {
            value = keyedArgs[key];
            return true;
        }
        return false;
    }

    public string Print()
    {
        if (printCallback != null)
        {
            return printCallback(this);
        }
        else
        {
            var builder = new StringBuilder();
            foreach (var argument in Positionals)
            {
                builder.Append(argument.ToString());
                builder.Append(Environment.NewLine);
            }
            foreach (var argument in Keys)
            {
                builder.Append(argument.ToString());
                builder.Append(Environment.NewLine);
            }
            foreach (var argument in Flags)
            {
                builder.Append(argument.ToString());
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }

    public bool HasFlag(string key) => flagArgs.Contains(key);

    public void LoadArgs(IEnumerable<string> args)
    {
        keyedArgs.Clear();
        positionalArgs.Clear();
        flagArgs.Clear();

        var listArgs = args.ToList();
        foreach (var arg in Keys)
        {
            var index = listArgs.FindIndex(0, listArgs.Count, a => a.Equals(arg.Key));
            if (index >= 0 && index <= listArgs.Count - 2)
            {
                keyedArgs.Add((string)arg.Key, listArgs[index + 1]);
                listArgs.RemoveAt(index + 1);
            }
            listArgs.RemoveAll(s => s.Equals(arg.Key));
        }
        foreach (var arg in Flags)
        {
            if (listArgs.Contains(arg.Key))
            {
                listArgs.RemoveAll(s => s.Equals(arg.Key));
                flagArgs.Add((string)arg.Key);
            }
        }
        positionalArgs.AddRange(listArgs);
    }

    public bool Valid() => Positionals.Count == positionalArgs.Count;

    public struct Argument : IEquatable<Argument>
    {
        public readonly object Key { get; init; }

        public readonly string Description { get; init; }

        public static bool operator ==(Argument left, Argument right) => left.Equals(right);

        public static bool operator !=(Argument left, Argument right) => !(left == right);

        public bool Equals(Argument other) => Key.Equals(other.Key);

        public override bool Equals(object? obj) => obj is Argument argument && Equals(argument);

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => Key + "\t" + Description;
    }

    public class Builder
    {
        private readonly ArgumentsHandler handler;

        public Builder()
        {
            handler = new ArgumentsHandler();
        }

        public static implicit operator ArgumentsHandler(Builder builder) => builder.Build();

        public Builder Positional(string description = "")
        {
            var arg = new Argument() { Key = handler.Positionals.Count, Description = description };
            handler.Positionals.Add(arg);
            return this;
        }

        public Builder Keyed(string key, string description = "")
        {
            var arg = new Argument() { Key = key, Description = description };
            handler.Keys.Add(arg);
            return this;
        }

        public Builder Print(Func<ArgumentsHandler, string> callback)
        {
            handler.printCallback = callback;
            return this;
        }

        public Builder Flag(string flag, string description = "")
        {
            var arg = new Argument() { Key = flag, Description = description };
            handler.Flags.Add(arg);
            return this;
        }

        public ArgumentsHandler Build() => handler;
    }
}