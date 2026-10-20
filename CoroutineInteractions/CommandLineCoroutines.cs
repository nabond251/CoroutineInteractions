using System.Threading.Channels;

namespace CoroutineInteractions;

public static class CommandLineCoroutines
{
    public static async Task GreetAsync(
        ChannelWriter<CommandLineInstruction> requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter your name."));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        var s = await readLineRequest.Responses.Reader.ReadAsync();

        await requestWriter.WriteAsync(new WriteLine($"Hello, {s}!"));

        requestWriter.Complete();
    }
}
