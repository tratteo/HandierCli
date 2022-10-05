// Copyright Matteo Beltrame

using System.Text;
using System.Text.RegularExpressions;

namespace HandierCli.CLI;

public partial class ArgumentsHandler
{
    [Serializable]
    public struct Argument : IEquatable<Argument>
    {
        private readonly Regex? regex;
        private readonly List<string>? allowedValues;

        public object Key { get; private set; }

        public string Description { get; private set; }

        public Argument(object key, string description, string? regexPattern, params string[] allowedValues)
        {
            Key = key;
            Description = description;
            regexPattern ??= "";
            regex = string.IsNullOrWhiteSpace(regexPattern) ? null : new Regex(regexPattern);
            this.allowedValues = null;
            if (allowedValues.Length > 0)
            {
                this.allowedValues = new List<string>();
                this.allowedValues.AddRange(allowedValues);
            }
        }

        public static bool operator ==(Argument left, Argument right) => left.Equals(right);

        public static bool operator !=(Argument left, Argument right) => !(left == right);

        public bool Equals(Argument other) => Key.Equals(other.Key);

        public override bool Equals(object? obj) => obj is Argument argument && Equals(argument);

        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// </summary>
        /// <param name="arg"> </param>
        /// <returns> Whether the provided string fits in the argument structure </returns>
        public bool Fits(string arg) => allowedValues is not null ? allowedValues.Contains(arg) : regex is not null ? regex.IsMatch(arg) : true;

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
}