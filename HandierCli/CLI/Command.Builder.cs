// Copyright Matteo Beltrame

using HandierCli.Log;

namespace HandierCli.CLI;

public partial class Command
{
    public class Builder
    {
        private readonly Command command;
        private ArgumentsHandler.Builder argBuilder;

        public Builder(string key)
        {
            command = new Command(key);
            argBuilder = new ArgumentsHandler.Builder();
        }

        public static implicit operator Command(Builder builder) => builder.Build();

        public Command Build()
        {
            command.argsHandler = argBuilder.Build();
            return command;
        }

        /// <summary>
        ///   Set the <see cref="ArgumentsHandler"/> for the command
        /// </summary>
        /// <param name="handler"> </param>
        /// <returns> </returns>
        public Builder WithArguments(ArgumentsHandler.Builder handler)
        {
            argBuilder = handler;
            return this;
        }

        /// <summary>
        ///   Override the help logger for the command, that will be used when printing the help (usage) of the command
        /// </summary>
        /// <param name="logger"> </param>
        /// <param name="color"> </param>
        /// <returns> </returns>
        public Builder HelpLogger(AdvancedLogger logger, ConsoleColor? color = null)
        {
            command.logger = logger;
            command.consoleColor = color;
            return this;
        }

        /// <summary>
        ///   Override the default print function (usage) for the command
        /// </summary>
        /// <param name="callback"> </param>
        /// <returns> </returns>
        public Builder Print(Func<Command, string> callback)
        {
            command.printCallback = callback;
            return this;
        }

        /// <summary>
        ///   Add an async callback that will be awaited in the internal execution. All async callbacks are executed in parallel
        /// </summary>
        /// <param name="task"> </param>
        /// <returns> </returns>
        public Builder AddAsync(Func<ArgumentsHandler, Task> task)
        {
            command.asyncActions.Add(task);
            return this;
        }

        /// <summary>
        ///   Add a normal callback to the command. All sync callbacks are executed sequentially
        /// </summary>
        /// <param name="task"> </param>
        /// <returns> </returns>
        public Builder Add(Action<ArgumentsHandler> task)
        {
            command.syncActions.Add(task);
            return this;
        }

        /// <summary>
        ///   Define a description for the command
        /// </summary>
        /// <param name="description"> </param>
        /// <returns> </returns>
        public Builder Description(string description)
        {
            command.Description = description;
            return this;
        }

        /// <summary>
        ///   Define a custom help flag for the command, used to display the usage
        /// </summary>
        /// <param name="flag"> </param>
        /// <returns> </returns>
        public Builder HelpFlag(string flag)
        {
            argBuilder.Flag(flag, "help");
            command.helpFlag = flag;
            return this;
        }
    }
}