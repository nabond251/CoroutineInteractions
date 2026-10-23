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
        var coroutine = WizardCoroutines.TryReserveAsync(writer);
        var sut = requestChannel.Reader.ReadAllAsync().GetAsyncEnumerator();

        Assert.True(await sut.MoveNextAsync());
        var instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Please enter number of diners:", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                async readLine =>
                {
                    await readLine.Responses.Writer.WriteAsync("4");
                },
                writeLine => Assert.Fail()),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Please enter your desired date:", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                async readLine =>
                {
                    await readLine.Responses.Writer.WriteAsync("2017-11-25 18-30-00Z");
                },
                writeLine => Assert.Fail()),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Not a date.", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Please enter your desired date:", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                async readLine =>
                {
                    await readLine.Responses.Writer.WriteAsync("2017-11-25 18:30:00Z");
                },
                writeLine => Assert.Fail()),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => Assert.Fail(),
            reservationsApiInstruction => reservationsApiInstruction.Switch(
                async getSlots =>
                {
                    Assert.Equal(DateTimeOffset.Parse("2017-11-25 18:30:00Z"), getSlots.Request);
                    await getSlots.Responses.Writer.WriteAsync(
                    [
                        new() { SlotDate = DateTimeOffset.Parse("2017-11-25"), SeatsLeft = 4 },
                    ]);
                },
                postReservation => Assert.Fail()));

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Please enter your name:", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                async readLine =>
                {
                    await readLine.Responses.Writer.WriteAsync("Mark Seemann");
                },
                writeLine => Assert.Fail()),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Please enter your email address:", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                async readLine =>
                {
                    await readLine.Responses.Writer.WriteAsync("mark@example.org");
                },
                writeLine => Assert.Fail()),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => Assert.Fail(),
            reservationsApiInstruction => reservationsApiInstruction.Switch(
                getSlots => Assert.Fail(),
                postReservation =>
                {
                    var reservation = postReservation.Request;
                    Assert.Equal(
                        DateTimeOffset.Parse("2017-11-25 18:30:00Z"),
                        reservation.Date);
                    Assert.Equal("Mark Seemann", reservation.Name);
                    Assert.Equal("mark@example.org", reservation.Email);
                    Assert.Equal(4, reservation.Quantity);
                }));

        requestChannel.Writer.Complete();
        Assert.False(await sut.MoveNextAsync());
    }

    [Fact]
    public async Task PreventsOverbookedReservationAsync()
    {
        var requestChannel = Channel.CreateUnbounded<WizardInstruction>();
        var writer = new WizardInstructionWriter(requestChannel.Writer);
        var coroutine = WizardCoroutines.TryReserveAsync(writer);
        var sut = requestChannel.Reader.ReadAllAsync().GetAsyncEnumerator();

        Assert.True(await sut.MoveNextAsync());
        var instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Please enter number of diners:", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                async readLine =>
                {
                    await readLine.Responses.Writer.WriteAsync("four");
                },
                writeLine => Assert.Fail()),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Not an integer.", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Please enter number of diners:", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                async readLine =>
                {
                    await readLine.Responses.Writer.WriteAsync("4");
                },
                writeLine => Assert.Fail()),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Please enter your desired date:", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                async readLine =>
                {
                    await readLine.Responses.Writer.WriteAsync("2017-11-25 18:30:00Z");
                },
                writeLine => Assert.Fail()),
            reservationsApiInstruction => Assert.Fail());

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => Assert.Fail(),
            reservationsApiInstruction => reservationsApiInstruction.Switch(
                async getSlots =>
                {
                    Assert.Equal(DateTimeOffset.Parse("2017-11-25 18:30:00Z"), getSlots.Request);
                    await getSlots.Responses.Writer.WriteAsync(
                    [
                        new() { SlotDate = DateTimeOffset.Parse("2017-11-25"), SeatsLeft = 2 },
                        new() { SlotDate = DateTimeOffset.Parse("2017-11-25"), SeatsLeft = 1 },
                    ]);
                },
                postReservation => Assert.Fail()));

        Assert.True(await sut.MoveNextAsync());
        instruction = sut.Current;
        instruction.Switch(
            commandLineInstruction => commandLineInstruction.Switch(
                readLine => Assert.Fail(),
                writeLine => Assert.Equal(
                    "Only 3 remaining seats.", writeLine.Request)),
            reservationsApiInstruction => Assert.Fail());

        requestChannel.Writer.Complete();
        Assert.False(await sut.MoveNextAsync());
    }
}
