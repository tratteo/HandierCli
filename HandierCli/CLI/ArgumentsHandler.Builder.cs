// Copyright Matteo Beltrame

namespace HandierCli.CLI;

public partial class ArgumentsHandler
{
    public class Builder
    {
        private readonly ArgumentsHandler handler;

        public Builder()
        {
            handler = new ArgumentsHandler();
        }

        public static implicit operator ArgumentsHandler(Builder builder) => builder.Build();

        /// <summary>
        ///   Add a mandatory parameter, at the ordered position
        /// </summary>
        /// <param name="description"> </param>
        /// <param name="regexPattern"> The regex pattern the mandatory argument must satisfy </param>
        /// <returns> </returns>
        public Builder Mandatory(string description, string? regexPattern = null)
        {
            var arg = new Argument(handler.Positionals.Count, description, regexPattern);
            handler.Positionals.Add(arg);
            return this;
        }

        /// <summary>
        ///   <inheritdoc cref="Mandatory(string, string?)"/>
        /// </summary>
        /// <param name="description"> </param>
        /// <param name="allowedValues"> Discrete set of allowed values </param>
        /// <returns> </returns>
        public Builder Mandatory(string description, string[] allowedValues)
        {
            var arg = new Argument(handler.Positionals.Count, description, null, allowedValues);
            handler.Positionals.Add(arg);
            return this;
        }

        /// <summary>
        ///   Add an optional unordered parameter that can be retrieved with the key
        /// </summary>
        /// <param name="description"> </param>
        /// <param name="regexPattern"> The regex pattern the mandatory argument must satisfy </param>
        /// <returns> </returns>
        public Builder Keyed(string key, string description, string? regexPattern = null)
        {
            var arg = new Argument(key, description, regexPattern);
            handler.Keys.Add(arg);
            return this;
        }

        /// <summary>
        ///   <inheritdoc cref=" Keyed(string, string, string?)"/>
        /// </summary>
        /// <param name="description"> </param>
        /// <param name="allowedValues"> Discrete set of allowed values </param>
        /// <returns> </returns>
        public Builder Keyed(string key, string description, string[] allowedValues)
        {
            var arg = new Argument(key, description, null, allowedValues);
            handler.Keys.Add(arg);
            return this;
        }

        /// <summary>
        ///   Add an unordered flag parameter, that is: it is either present or not
        /// </summary>
        /// <param name="description"> </param>
        /// <param name="regexPattern"> The regex pattern the mandatory argument must satisfy </param>
        /// <returns> </returns>
        public Builder Flag(string flag, string description, string? regexPattern = null)
        {
            var arg = new Argument(flag, description, regexPattern);
            handler.Flags.Add(arg);
            return this;
        }

        /// <summary>
        ///   <inheritdoc cref="Flag(string, string, string?)"/>
        /// </summary>
        /// <param name="description"> </param>
        /// <param name="allowedValues"> Discrete set of allowed values </param>
        /// <returns> </returns>
        public Builder Flag(string flag, string description, string[] allowedValues)
        {
            var arg = new Argument(flag, description, null, allowedValues);
            handler.Flags.Add(arg);
            return this;
        }

        /// <summary>
        ///   Set a custom print function, used for displaying the structure of the handler. The default is already formatted
        /// </summary>
        /// <param name="callback"> </param>
        /// <returns> </returns>
        public Builder Print(Func<ArgumentsHandler, string> callback)
        {
            handler.printCallback = callback;
            return this;
        }

        public ArgumentsHandler Build() => handler;
    }
}