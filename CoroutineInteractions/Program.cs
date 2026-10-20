using CoroutineInteractions;
using System.Threading.Channels;

static async Task CommandLineCoroutineAsync(
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

var requestChannel = Channel.CreateUnbounded<CommandLineInstruction>();
var coroutine = CommandLineCoroutineAsync(requestChannel.Writer);

await foreach (var instruction in requestChannel.Reader.ReadAllAsync())
{
    instruction.Switch(
        readLine =>
        {
            var response = Console.ReadLine();
            var writer = readLine.Responses.Writer;
            writer.TryWrite(response);
            writer.Complete();
        },
        writeLine =>
        {
            var request = writeLine.Request;
            Console.WriteLine(request);
        });
}

await coroutine;
