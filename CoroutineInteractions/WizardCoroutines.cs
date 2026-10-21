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
            return dinerCount;
        }
        else
        {
            await requestWriter.WriteAsync(new WriteLine("Not an integer."));
            return await ReadQuantity(requestWriter);
        }
    }

    public static async Task<DateTimeOffset> ReadDate(
        ChannelWriter<CommandLineInstruction> requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter your desired date:"));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        var l = await readLineRequest.Responses.Reader.ReadAsync();

        if (DateTimeOffset.TryParse(l, out var dt))
        {
            return dt;
        }
        else
        {
            await requestWriter.WriteAsync(new WriteLine("Not a date."));
            return await ReadDate(requestWriter);
        }
    }

    public static async Task<string?> ReadName(
        ChannelWriter<CommandLineInstruction> requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter your name:"));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        return await readLineRequest.Responses.Reader.ReadAsync();
    }

    public static async Task<string?> ReadEmail(
        ChannelWriter<CommandLineInstruction> requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter your email address:"));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        return await readLineRequest.Responses.Reader.ReadAsync();
    }
}
