using CoroutineInteractions.ReservationsApi;

namespace CoroutineInteractions;

public static class WizardCoroutines
{
    public static async Task<int> ReadQuantityAsync(
        ICommandLine commandLine)
    {
        await commandLine.WriteLineAsync("Please enter number of diners:");
        var l = await commandLine.ReadLineAsync();

        if (int.TryParse(l, out var dinerCount))
        {
            return dinerCount;
        }
        else
        {
            await commandLine.WriteLineAsync("Not an integer.");
            return await ReadQuantityAsync(commandLine);
        }
    }

    public static async Task<DateTimeOffset> ReadDateAsync(
        ICommandLineInstructionWriter requestWriter)
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
        ICommandLineInstructionWriter requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter your name:"));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        return await readLineRequest.Responses.Reader.ReadAsync();
    }

    public static async Task<string?> ReadEmailAsync(
        ICommandLineInstructionWriter requestWriter)
    {
        await requestWriter.WriteAsync(
            new WriteLine("Please enter your email address:"));

        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        return await readLineRequest.Responses.Reader.ReadAsync();
    }

    public static async Task TryReserveAsync(
        ICommandLine commandLine,
        IWizardInstructionWriter requestWriter)
    {
        var count = await ReadQuantityAsync(commandLine);
        var date = await ReadDateAsync(requestWriter);

        var getSlotsRequest = new GetSlots(date);
        await requestWriter.WriteAsync(getSlotsRequest);
        var availableSeats =
            (await getSlotsRequest.Responses.Reader.ReadAsync())
            .Sum(s => s.SeatsLeft);

        if (availableSeats < count)
        {
            await requestWriter.WriteAsync(
                new WriteLine($"Only {availableSeats} remaining seats."));
        }
        else
        {
            var name = await ReadNameAsync(requestWriter);
            var email = await ReadEmailAsync(requestWriter);
            var reservation = new Reservation
            {
                Date = date,
                Name = name,
                Email = email,
                Quantity = count,
            };
            await requestWriter.WriteAsync(new PostReservation(reservation));
        }
    }
}
