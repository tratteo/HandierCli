![Nuget](https://img.shields.io/nuget/v/HandierCli?label=NuGet)
![GitHub](https://img.shields.io/github/license/tratteo/HandierCli?color=orange&label=License)
![GitHub top language](https://img.shields.io/github/languages/top/tratteo/HandierCli?color=5027d5&label=C%23&logo=.net)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/tratteo/HandierCli/main?label=Last%20commit&color=brightgreen&logo=github)

# HandierCli
A package that allows for fast creation of command line interface functionalities.

-----

## Quick Start
  [CLI](#cli)
- [Creating the CLI](#creating-the-cli)
- [Commands](#commands)
  - [Command callbacks](#command-callbacks)
  - [Arguments](#arguments)
    - [Mandatory arguments](#mandatory-arguments)
    - [Optional arguments](#optional-arguments)
    - [Flags](#flag-arguments)
    - [Domains](#arguments-domain)
    
 [Console progress](#console-progress)
 - [Progress bar](#progress-bar)
 - [Spinner](#spinner)

### CLI

#### Creating the CLI

```c#
// Create the CLI with a builder pattern, use Intellisense for more methods
CommandLine cli = CommandLine.Factory()   
         // Set what to do when a command is not recognized (not registered), see below registration
        .OnUnrecognized((logger, cmd) => logger.Log($"{cmd} not recognized", ConsoleColor.DarkRed))   
        // Register the default help command, when typing help, all the registered commands and their usage will be displayed
        .RegisterHelpCommand()
        // Set a default global help symbol, when typing the help symbol after a command, its usage will be displayed
        .GlobalHelpSymbol("-h")
        .Build();

await cli.RunAsync();
```

### Commands

Commands are registered with the function `cli.RegisterCommand(Command.Builder command)`. Commands are created using the builder pattern.

Register a command that runs when **usefulcmd** is typed in the CLI.
```c#
cli.Register(Command.Factory("usefulcmd")
    .Description("execute an extremely useful piece of code")
    .Add(handler =>
    {
        // Useful code goes here
    }));
```

#### Command callbacks 
Commands work with callback. Callbacks are added to the builder pattern of the Command.
They can be added with the  `Add(...)` method or with the `AddAsync(...)` method.
- When added in the normal version, commands are executed sequentially in an order determined by the registration order
- When added in the async way, commands are executed in parallel and awaited

### Arguments 
The arguments of a `Command` are defined by its `ArgumentsHandler`. Omitting the ArgumentsHandler will create a command with no arguments that runs simply when typing it into the CLI.   
Guess what, ArgumentsHandler are build with the builder pattern too.     
The three types of arguments can of course be mixed.
#### Mandatory arguments
Mandatory (positional) arguments must be provided and are position sensitive. Their order is defined by the order in which they are registered in the builder pattern.

#### Mandatory arguments

Register a command that runs when **usefulcmd** is typed in the CLI with a *mandatory* argument in first position.
```c#
cli.Register(Command.Factory("usefulcmd")
    .Description("execute an extremely useful piece of code")
    .WithArguments(ArgumentsHandler.Factory()
        .Mandatory("a mandatory argument")
    .Add(handler =>
    {
        string arg = handler.GetPositional(0);
        // Useful code goes here
    }));
```

Register a command that runs when **usefulcmd** is typed in the CLI with two *mandatory* arguments.
```c#
cli.Register(Command.Factory("usefulcmd")
    .Description("execute an extremely useful piece of code")
    .WithArguments(ArgumentsHandler.Factory()
        .Mandatory("a mandatory argument")
        .Mandatory("another mandatory argument")
    .Add(handler =>
    {
        string arg = handler.GetPositional(0);
        string arg1 = handler.GetPositional(1);
        // Useful code goes here
    }));
```

#### Optional arguments
Keys are optional arguments. Keys are defined with a *key*. The key can be used to retrieve the value of the provided argument in the callback.

Register a command that runs when **usefulcmd** is typed in the CLI with an optional argument.
```c#
cli.Register(Command.Factory("usefulcmd")
    .Description("execute an extremely useful piece of code")
    .WithArguments(ArgumentsHandler.Factory()
        .Keyed("-k", "an optional keyed argument")
    .Add(handler =>
    {
        if(handler.TryGetKeyed("-k", out string val)
        {
            // Key was provided
        }
        else
        {
            // Key was not provided :(
        }
    }));
```

#### Flag arguments
Flags are arguments with no value that are useful to detect binary conditions.

Register a command that runs when **usefulcmd** is typed in the CLI with a flag.
```c#
cli.Register(Command.Factory("usefulcmd")
    .Description("execute an extremely useful piece of code")
    .WithArguments(ArgumentsHandler.Factory()
        .Flag("/f", "a useful flag")
    .Add(handler =>
    {
        if(handler.HasFlag("/k"))
        {
            // Flag was provided
        }
        else
        {
            // Flag was not provided :(
        }
    }));
```


#### Arguments domain
It is possible to define domains for the arguments. In case the provided string for the argument does not belong to the domain, the CLI will print an error and display the usage of the command the argument is referred to.

Domains are defined as follows:
- **Value collection**: in this case when adding an argument, provide also an array of possible option for the argument value
- **Regular expressions**: in this case, a regular expression logic will be run on the argument value

Register a command that runs when **usefulcmd** is typed in the CLI with two *mandatory* arguments with two different domains.
```c#
cli.Register(Command.Factory("usefulcmd")
    .Description("execute an extremely useful piece of code")
    .WithArguments(ArgumentsHandler.Factory()
        .Mandatory("a mandatory argument", new string[] { "option1", "option2", "option3" })
        .Mandatory("another mandatory argument", ".json$")
    .Add(handler =>
    {
        string arg = handler.GetPositional(0);
        string arg1 = handler.GetPositional(1);
        // Useful code goes here, argument values are assured to be contained into the defined domain
    }));
```

------------

### Console progress
HandierCli also provides some useful constructs to easily manage progress report in the console.

#### Progress bar
Progress bar is used to report works that are able to report a progress in the interval `[0, 1]`.
The following example shows how to create and use the progress bar:
```c#
var bar = ConsoleProgressBar.Factory()
    // Customize the style :D
    .Style('X', ' ', '[', ']')
    // Set the lenght
    .Lenght(50)
    // Start the progress bar rendering as soon as the Build() is called otherwise it can be started when wanted with Start()
    .StartOnBuild()
    // Extremely long but very cool shark animation :D
    .Spinner(8, "||\\________|",
                "|_|\\_______|",
                "|__|\\______|",
                "|___|\\_____|",
                "|____|\\____|",
                "|_____|\\___|",
                "|______|\\__|",
                "|_______|\\_|",
                "|________|\\|",
                "|________/||",
                "|_______/|_|",
                "|______/|__|",
                "|_____/|___|",
                "|____/|____|",
                "|___/|_____|",
                "|__/|______|",
                "|_/|_______|",
                "|/|________|")
    .Build();

// Progress bar implements IDisposable, otherwise dispose it manually with Dispose()
using (bar)
{
    int length = 100;
    for (int i = 0; i < length; i++)
    {
        bar.Report((float)i / (length - 1));
        await Task.Delay(50);
    }
}
```

#### Spinner
The spinner is used to show the user that a job is working, whenever a progress value is not available.
The following examples show how to use the spinner class:
```c#
// Build the spinner
var spinner = ConsoleSpinner.Factory()
    // Set the default info text
    .Info("I am doing a very heavy job X(   ")
    // Set the default completed text
    .Completed("Done :D")
    .Frames(12, "|o        |",
                "| o       |",
                "|  o      |",
                "|   o     |",
                "|    o    |",
                "|     o   |",
                "|      o  |",
                "|       o |",
                "|        o|",
                "|       o |",
                "|      o  |",
                "|     o   |",
                "|    o    |",
                "|   o     |",
                "|  o      |",
                "| o       |")
    .Build();

using (spinner)
{
    spinner.Start();
    await Task.Delay(2000);
    spinner.Completed();

    await Task.Delay(500);

    // Await a dummy task
    await spinner.Await(Task.Run(async () => await Task.Delay(2000)));

    await Task.Delay(500);

    // Spinner can await task with return types
    var res = await spinner.Await(Task.Run(async () =>
    {
        // Spinner supports custom reports, overriding the default info text
        await Task.Delay(1000);
        spinner.Report("Step 1  ");
        await Task.Delay(1000);
        spinner.Report("Step 2  ");
        await Task.Delay(1000);
        return 10;
    }));
}
```
