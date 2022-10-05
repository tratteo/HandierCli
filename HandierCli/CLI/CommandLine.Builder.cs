// Copyright Matteo Beltrame

namespace HandierCli.CLI;

public partial class CommandLine
{
    public class Builder
    {
        private readonly CommandLine commandLine;
        private Command.Builder? helpCommandOverride;

        public Builder()
        {
            commandLine = new CommandLine();
        }

        public static implicit operator CommandLine(Builder builder) => builder.Build();

        /// <summary>
        ///   Define custom string that can be used to close the CLI
        /// </summary>
        /// <param name="exitCommands"> </param>
        /// <returns> </returns>
        public Builder ExitOn(params string[] exitCommands)
        {
            commandLine.exitCommands.AddRange(exitCommands);
            return this;
        }

        /// <summary>
        ///   Define the symbol that is printed at each line
        /// </summary>
        /// <param name="symbol"> </param>
        /// <returns> </returns>
        public Builder CliSymbol(string symbol)
        {
            commandLine.symbol = symbol;
            return this;
        }

        /// <summary>
        ///   Set a global help symbol, overrides the internal help symbol for all the registered <see cref="Command"/>, so that each one of
        ///   them has the same flag
        /// </summary>
        /// <param name="symbol"> </param>
        /// <returns> </returns>
        public Builder GlobalHelpSymbol(string symbol)
        {
            commandLine.globalHelpSymbol = symbol;
            return this;
        }

        /// <summary>
        ///   Customize the appearance of the CLI
        /// </summary>
        /// <param name="symbol"> </param>
        /// <param name="helpColor"> </param>
        /// <returns> </returns>
        public Builder Customize(string? symbol = null, ConsoleColor? helpColor = null)
        {
            if (symbol is not null) commandLine.symbol = symbol;
            if (helpColor is not null) commandLine.helpColor = (ConsoleColor)helpColor;
            return this;
        }

        /// <summary>
        ///   Register an help command, the default one displays and prints all the registered commands with their usage
        /// </summary>
        /// <param name="helpCommandOverride"> </param>
        /// <returns> </returns>
        public Builder RegisterHelpCommand(Command.Builder? helpCommandOverride = null)
        {
            this.helpCommandOverride = helpCommandOverride;
            return this;
        }

        /// <summary>
        ///   Specify a callback to be used when a command is not recognized
        /// </summary>
        /// <param name="callback"> </param>
        /// <returns> </returns>
        public Builder OnUnrecognized(Action<Logger, string> callback)
        {
            commandLine.onUnrecognized = callback;
            return this;
        }

        public CommandLine Build()
        {
            helpCommandOverride ??= Command.Factory("help")
                    .Description("display the available commands")
                    .Add((handler) => commandLine.CliLogger.Log(commandLine.Print(), commandLine.helpColor));
            helpCommandOverride.HelpLogger(commandLine.CliLogger, commandLine.helpColor);
            if (!commandLine.commands.Contains(helpCommandOverride))
            {
                commandLine.commands.Add(helpCommandOverride);
            }
            return commandLine;
        }
    }
}