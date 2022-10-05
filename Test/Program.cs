using HandierCli.CLI;
using Microsoft.Extensions.Logging;

//var a = ArgumentsHandler.Factory().Positional("test p", "^[Aa-zZ].json$").Build();
//a.LoadArgs(new string[] { "s.json" });
//var res = a.Fits();
//Console.WriteLine(JsonSerializer.Serialize(res, new JsonSerializerOptions() { WriteIndented = true }));
CommandLine cli = CommandLine.Factory().OnUnrecognized((l, s) => l.LogInformation(s)).RegisterHelpCommand().GlobalHelpSymbol("--h").Customize(null, ConsoleColor.Magenta);
cli.Register(Command.Factory("test").WithArguments(ArgumentsHandler.Factory().Mandatory("useless param", "[a-zA-Z]{3}.txt").Keyed("-l", "usefult key", new string[] { "test1", "val" })));
await cli.RunAsync();