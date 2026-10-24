using System.Threading.Channels;

namespace CoroutineInteractions.Tests;

public class ClcTests
{
    [Theory]
    [InlineData("")]
    [InlineData("Mark")]
    [InlineData("Nathan")]
    public async Task GreetsUserAsync(string username)
    {
        var requestChannel = Channel.CreateUnbounded<CommandLineInstruction>();
        var writer = new CommandLineInstructionWriter(requestChannel.Writer);
        var cli = new CoroutineCommandLine(writer);
        var coroutine = CommandLineCoroutines.GreetAsync(cli);
        var sut = requestChannel.Reader.ReadAllAsync().GetAsyncEnumerator();

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter your name.");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine(username);

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine($"Hello, {username}!");

        requestChannel.Writer.Complete();
        Assert.False(await sut.MoveNextAsync());
        await coroutine;
    }
}
