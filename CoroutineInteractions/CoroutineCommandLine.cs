namespace CoroutineInteractions;

public class CoroutineCommandLine(
    ICommandLineInstructionWriter requestWriter) : ICommandLine
{
    public async Task<string?> ReadLineAsync()
    {
        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        return await readLineRequest.Responses.Reader.ReadAsync();
    }

    public async Task WriteLineAsync(string text)
    {
        await requestWriter.WriteAsync(new WriteLine(text));
    }
}
