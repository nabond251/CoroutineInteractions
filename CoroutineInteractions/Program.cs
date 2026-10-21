using CoroutineInteractions;
using System.Threading.Channels;

var requestChannel = Channel.CreateUnbounded<CommandLineInstruction>();
var countCoroutine = WizardCoroutines.ReadQuantity(requestChannel.Writer);
var dateCoroutine = WizardCoroutines.ReadQuantity(requestChannel.Writer);

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

var count = await countCoroutine;
var date = await dateCoroutine;
