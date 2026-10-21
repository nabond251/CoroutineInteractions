using System.Threading.Channels;

namespace CoroutineInteractions;

public static class WizardCoroutines
{
    public static async Task<int> ReadQuantityAsync(
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
            return await ReadQuantityAsync(requestWriter);
        }
    }

    public static async Task<DateTimeOffset> ReadDateAsync(
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
            return await ReadDateAsync(requestWriter);
        }
    }

    public static async Task<string?> ReadNameAsync(
        ChannelWriter<CommandLineInstruction> requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter your name:"));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        return await readLineRequest.Responses.Reader.ReadAsync();
    }

    public static async Task<string?> ReadEmailAsync(
        ChannelWriter<CommandLineInstruction> requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter your email address:"));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        return await readLineRequest.Responses.Reader.ReadAsync();
    }

    public static async Task<Reservation> ReadReservationRequestAsync(
        ChannelWriter<CommandLineInstruction> requestWriter)
    {
        var count = await ReadQuantityAsync(requestWriter);
        var date = await ReadDateAsync(requestWriter);
        var name = await ReadNameAsync(requestWriter);
        var email = await ReadEmailAsync(requestWriter);
        return new(date, name, email, count);
    }
}
