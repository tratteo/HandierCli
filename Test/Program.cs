using HandierCli.CLI;
using HandierCli.Progress;
using Microsoft.Extensions.Logging;

//var a = ArgumentsHandler.Factory().Positional("test p", "^[Aa-zZ].json$").Build();
//a.LoadArgs(new string[] { "s.json" });
//var res = a.Fits();
//Console.WriteLine(JsonSerializer.Serialize(res, new JsonSerializerOptions() { WriteIndented = true }));
//CommandLine cli = CommandLine.Factory().OnUnrecognized((l, s) => l.LogInformation(s)).RegisterHelpCommand().GlobalHelpSymbol("--h").Customize(null, ConsoleColor.Magenta);
//cli.Register(
//    Command.Factory("test")
//    .WithArguments(ArgumentsHandler.Factory()
//    .Mandatory("useless param", "[a-zA-Z]{3}.txt")
//    .Keyed("-l", "usefult key", new string[] { "test1", "Val" }))
//    .Add(h =>
//    {
//        Console.WriteLine("RUN");
//    }));
//await cli.RunAsync();

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