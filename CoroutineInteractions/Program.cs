using CoroutineInteractions;
using System.Threading.Channels;

var requestChannel = Channel.CreateUnbounded<CommandLineInstruction>();

var coroutine = Task.Run(async () =>
{
    var reservation = await WizardCoroutines.ReadReservationRequestAsync(
        requestChannel.Writer);
    requestChannel.Writer.Complete();
    return reservation;
});

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

Console.WriteLine(await coroutine);
