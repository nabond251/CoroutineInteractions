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
        var coroutine = CommandLineCoroutines.GreetAsync(requestChannel.Writer);
        var sut = requestChannel.Reader.ReadAllAsync().GetAsyncEnumerator();

        Assert.True(await sut.MoveNextAsync());
        var instruction = sut.Current;
        instruction.Switch(
            readLine => Assert.Fail(),
            writeLine => Assert.Equal(
                "Please enter your name.", writeLine.Request));

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            async readLine =>
            {
                await readLine.Responses.Writer.WriteAsync(username);
            },
            writeLine => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            readLine => Assert.Fail(),
            writeLine => Assert.Equal(
                $"Hello, {username}!", writeLine.Request));

        Assert.False(await sut.MoveNextAsync());
    }
}
