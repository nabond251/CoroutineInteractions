namespace CoroutineInteractions;

public static class CommandLineCoroutines
{
    public static async Task GreetAsync(ICommandLine commandLine)
    {
        await commandLine.WriteLineAsync("Please enter your name.");
        var s = await commandLine.ReadLineAsync();
        await commandLine.WriteLineAsync($"Hello, {s}!");
    }
}
