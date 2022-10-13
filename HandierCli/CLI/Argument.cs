// Copyright Matteo Beltrame

using System.Text;
using System.Text.RegularExpressions;

namespace HandierCli.CLI;

[Serializable]
public struct Argument : IEquatable<Argument>
{
    private readonly Regex? regex;
    private readonly List<string>? allowedValues;
    private readonly List<string>? invariantLowerCase;
    private bool caseSensitive = true;

    public object Key { get; private set; }

    public string Description { get; private set; }

    public Argument(object key, string description, string? regexPattern, bool caseSensitive = false, params string[] allowedValues)
    {
        Key = key;
        Description = description;
        this.caseSensitive = caseSensitive;
        regexPattern ??= "";
        regex = string.IsNullOrWhiteSpace(regexPattern) ? null : new Regex(regexPattern);
        this.allowedValues = null;
        this.invariantLowerCase = null;
        if (allowedValues.Length > 0)
        {
            this.allowedValues = new List<string>();
            this.allowedValues.AddRange(allowedValues);
            this.invariantLowerCase = new List<string>();
            this.invariantLowerCase.AddRange(this.allowedValues.ConvertAll(v => v.ToLowerInvariant()));
        }
    }

    /// <inheritdoc/>
    public static bool operator ==(Argument left, Argument right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(Argument left, Argument right) => !(left == right);

    /// <inheritdoc/>
    public bool Equals(Argument other) => Key.Equals(other.Key);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Argument argument && Equals(argument);

    /// <inheritdoc/>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>
    /// </summary>
    /// <param name="arg"> </param>
    /// <returns> Whether the provided string fits in the argument structure </returns>
    public bool Fits(string arg)
    {
        return regex is not null
            ? regex.IsMatch(arg)
            : allowedValues is null
            || (!caseSensitive && invariantLowerCase is not null
                ? invariantLowerCase.Contains(arg.ToLowerInvariant())
                : allowedValues.Contains(arg));
    }

    /// <summary>
    /// </summary>
    /// <returns> The argument string representation </returns>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(Key).Append('\t').Append(Description);

        if (allowedValues is not null)
        {
            builder.Append(" [");
            builder.Append(string.Join(", ", allowedValues));
            builder.Append("] ");
        }
        else if (regex is not null)
        {
            builder.Append(" r[\"");
            builder.Append(regex.ToString());
            builder.Append("\"] ");
        }
        return builder.ToString();
    }
}