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

    public static void AssertWriteLine(
        this CommandLineInstruction actual,
        Action<WriteLine> onWriteLine) =>
        actual.Switch(readLine => Assert.Fail(), onWriteLine);

    public static void AssertWriteLine(
        this CommandLineInstruction actual,
        string expected) =>
        actual.AssertWriteLine(writeLine => Assert.Equal(
            expected, writeLine.Request));
}
