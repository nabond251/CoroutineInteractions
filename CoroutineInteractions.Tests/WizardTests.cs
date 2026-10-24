using CoroutineInteractions.ReservationsApi;
using System.Threading.Channels;

namespace CoroutineInteractions.Tests;

public class WizardTests
{
    [Fact]
    public async Task PostsValidReservationAsync()
    {
        var requestChannel = Channel.CreateUnbounded<WizardInstruction>();
        var writer = new WizardInstructionWriter(requestChannel.Writer);
        var cli = new CoroutineCommandLine(writer);
        var api = new CoroutineReservationsApi(writer);
        var coroutine = WizardCoroutines.TryReserveAsync(cli, api);
        var sut = requestChannel.Reader.ReadAllAsync().GetAsyncEnumerator();

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter number of diners:");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine("4");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter your desired date:");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine("2017-11-25 18-30-00Z");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Not a date.");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter your desired date:");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine("2017-11-25 18:30:00Z");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertGetSlots(
            DateTimeOffset.Parse("2017-11-25 18:30:00Z"),
            [
                new() { SlotDate = DateTimeOffset.Parse("2017-11-25"), SeatsLeft = 4 },
            ]);

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter your name:");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine("Mark Seemann");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter your email address:");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine("mark@example.org");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertPostReservation(new()
        {
            Date = DateTimeOffset.Parse("2017-11-25 18:30:00Z"),
            Name = "Mark Seemann",
            Email = "mark@example.org",
            Quantity = 4,
        });

        requestChannel.Writer.Complete();
        Assert.False(await sut.MoveNextAsync());
    }

    [Fact]
    public async Task PreventsOverbookedReservationAsync()
    {
        var requestChannel = Channel.CreateUnbounded<WizardInstruction>();
        var writer = new WizardInstructionWriter(requestChannel.Writer);
        var cli = new CoroutineCommandLine(writer);
        var api = new CoroutineReservationsApi(writer);
        var coroutine = WizardCoroutines.TryReserveAsync(cli, api);
        var sut = requestChannel.Reader.ReadAllAsync().GetAsyncEnumerator();

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter number of diners:");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine("four");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Not an integer.");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter number of diners:");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine("4");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Please enter your desired date:");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertReadLine("2017-11-25 18:30:00Z");

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertGetSlots(
            DateTimeOffset.Parse("2017-11-25 18:30:00Z"),
            [
                new() { SlotDate = DateTimeOffset.Parse("2017-11-25"), SeatsLeft = 2 },
                new() { SlotDate = DateTimeOffset.Parse("2017-11-25"), SeatsLeft = 1 },
            ]);

        Assert.True(await sut.MoveNextAsync());
        sut.Current.AssertWriteLine("Only 3 remaining seats.");

        requestChannel.Writer.Complete();
        Assert.False(await sut.MoveNextAsync());
    }
}
