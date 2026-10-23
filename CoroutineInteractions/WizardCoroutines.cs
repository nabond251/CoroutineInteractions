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
        ICommandLine commandLine)
    {
        await commandLine.WriteLineAsync("Please enter your desired date:");
        var l = await commandLine.ReadLineAsync();

        if (DateTimeOffset.TryParse(l, out var dt))
        {
            return dt;
        }
        else
        {
            await commandLine.WriteLineAsync("Not a date.");
            return await ReadDateAsync(commandLine);
        }
    }

    public static async Task<string?> ReadNameAsync(
        ICommandLine commandLine)
    {
        await commandLine.WriteLineAsync("Please enter your name:");
        return await commandLine.ReadLineAsync();
    }

    public static async Task<string?> ReadEmailAsync(
        ICommandLine commandLine)
    {
        await commandLine.WriteLineAsync("Please enter your email address:");
        return await commandLine.ReadLineAsync();
    }

    public static async Task TryReserveAsync(
        ICommandLine commandLine,
        IWizardInstructionWriter requestWriter)
    {
        var count = await ReadQuantityAsync(commandLine);
        var date = await ReadDateAsync(commandLine);

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
            var name = await ReadNameAsync(commandLine);
            var email = await ReadEmailAsync(commandLine);
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
