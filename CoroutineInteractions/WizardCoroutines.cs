using System.Threading.Channels;

namespace CoroutineInteractions;

public static class WizardCoroutines
{
    public static async Task<int> ReadQuantity(
        ChannelWriter<CommandLineInstruction> requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter number of diners:"));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        var l = await readLineRequest.Responses.Reader.ReadAsync();

        if (int.TryParse(l, out var dinerCount))
        {
            requestWriter.Complete();
            return dinerCount;
        }
        else
        {
            await requestWriter.WriteAsync(new WriteLine("Not an integer."));
            return await ReadQuantity(requestWriter);
        }
    }
}
