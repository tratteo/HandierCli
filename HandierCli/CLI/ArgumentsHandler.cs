// Copyright Matteo Beltrame

using System.Text;

namespace HandierCli.CLI;

public partial class ArgumentsHandler
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

    /// <summary>
    /// </summary>
    /// <param name="index"> </param>
    /// <returns> The mandatory (positional) argument at the specified index </returns>
    public string GetPositional(int index) => positionalArgs[index];

    /// <summary>
    /// </summary>
    /// <param name="key"> </param>
    /// <param name="value"> </param>
    /// <returns> Whether the key could be retrieved </returns>
    public bool TryGetKeyed(string key, out string value)
    {
        value = string.Empty;
        if (keyedArgs.ContainsKey(key))
        {
            value = keyedArgs[key];
            return true;
        }
        return false;
    }

    /// <summary>
    ///   Default formatted printer for the handler
    /// </summary>
    /// <returns> </returns>
    public string Print()
    {
        var sep = new string('-', 50) + "\n";
        if (printCallback != null)
        {
            return printCallback(this);
        }
        else
        {
            var builder = new StringBuilder("\n");
            if (Positionals.Count > 0) builder.Append("Mandatory\n");
            foreach (var argument in Positionals)
            {
                builder.Append(argument.ToString());
                builder.Append(Environment.NewLine);
            }
            if (Positionals.Count > 0) builder.Append(sep);
            if (Keys.Count > 0) builder.Append("Optionals\n");
            foreach (var argument in Keys)
            {
                builder.Append(argument.ToString());
                builder.Append(Environment.NewLine);
            }
            if (Keys.Count > 0) builder.Append(sep);
            if (Flags.Count > 0) builder.Append("Flags\n");
            foreach (var argument in Flags)
            {
                builder.Append(argument.ToString());
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="key"> </param>
    /// <returns> Whether the flag is present </returns>
    public bool HasFlag(string key) => flagArgs.Contains(key);

    public FitResult Fits()
    {
        if (Positionals.Count < positionalArgs.Count) return FitResult.Failure("Too many mandatory parameters provided");
        if (Positionals.Count > positionalArgs.Count) return FitResult.Failure("Missing mandatory parameters");

        var failed = new List<(Argument, string)>();
        var success = true;
        foreach (var (val, arg) in positionalArgs.Zip(Positionals))
        {
            if (!arg.Fits(val))
            {
                success = false;
                failed.Add((arg, val));
            }
        }
        foreach (var (val, arg) in keyedArgs.Zip(Keys))
        {
            if (!arg.Fits(val.Value))
            {
                success = false;
                failed.Add((arg, val.Value));
            }
        }
        foreach (var (val, arg) in flagArgs.Zip(Flags))
        {
            if (!arg.Fits(val))
            {
                success = false;
                failed.Add((arg, val));
            }
        }
        return !success
            ? FitResult.Failure(failed, "Wrong arguments values")
            : FitResult.Success();
    }

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
}