using CoroutineInteractions.ReservationsApi;

namespace CoroutineInteractions.Tests;

public static class AssertionHelpers
{
    public static void AssertReadLine(
        this CommandLineInstruction actual,
        Action<ReadLine> onReadLine) =>
        actual.Switch(onReadLine, writeLine => Assert.Fail());

    public static void AssertReadLine(
        this CommandLineInstruction actual,
        string response) =>
        actual.AssertReadLine(async readLine =>
            await readLine.Responses.Writer.WriteAsync(response));

    public static void AssertReadLine(
        this WizardInstruction actual,
        string expected) =>
        actual.Switch(
            commandLineInstruction =>
            commandLineInstruction.AssertReadLine(expected),
            reservationsApiInstruction => Assert.Fail());

    public static void AssertWriteLine(
        this CommandLineInstruction actual,
        Action<WriteLine> onWriteLine) =>
        actual.Switch(readLine => Assert.Fail(), onWriteLine);

    public static void AssertWriteLine(
        this CommandLineInstruction actual,
        string expected) =>
        actual.AssertWriteLine(writeLine => Assert.Equal(
            expected, writeLine.Request));

    public static void AssertWriteLine(
        this WizardInstruction actual,
        string expected) =>
        actual.Switch(
            commandLineInstruction =>
            commandLineInstruction.AssertWriteLine(expected),
            reservationsApiInstruction => Assert.Fail());

    public static void AssertGetSlots(
        this WizardInstruction actual,
        DateTimeOffset expected,
        ICollection<Slot> response) =>
        actual.Switch(
            commandLineInstruction => Assert.Fail(),
            reservationsApiInstruction => reservationsApiInstruction.Switch(
                async getSlots =>
                {
                    Assert.Equal(expected, getSlots.Request);
                    await getSlots.Responses.Writer.WriteAsync(response);
                },
                postReservation => Assert.Fail()));

    public static void AssertPostReservation(
        this WizardInstruction actual,
        Reservation expected) =>
        actual.Switch(
            commandLineInstruction => Assert.Fail(),
            reservationsApiInstruction => reservationsApiInstruction.Switch(
                getSlots => Assert.Fail(),
                postReservation =>
                {
                    var reservation = postReservation.Request;
                    Assert.Equal(expected.Date, reservation.Date);
                    Assert.Equal(expected.Name, reservation.Name);
                    Assert.Equal(expected.Email, reservation.Email);
                    Assert.Equal(expected.Quantity, reservation.Quantity);
                }));
}
