using HandierCli;

var cli = CommandLine.Factory().Build();
_ = Test();
await cli.RunAsync();

async Task Test()
{
    await Task.Delay(1000);
    cli.CanReceiveCommands(false);
    await Task.Delay(5000);
    cli.CanReceiveCommands(true);
}