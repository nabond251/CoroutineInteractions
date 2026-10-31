namespace CoroutineInteractions;

public class CoroutineInteractor(
    IInteractorInstructionWriter requestWriter) : IInteractor
{
    public async Task<int> YieldParserAsync()
    {
        var yieldParserRequest = new YieldParser();
        await requestWriter.WriteAsync(yieldParserRequest);
        return await yieldParserRequest.Responses.Reader.ReadAsync();
    }

    public async Task YieldDecompressorAsync(int c)
    {
        await requestWriter.WriteAsync(new YieldDecompressor(c));
    }

    public async Task<int> ReadAsync()
    {
        var readRequest = new Read();
        await requestWriter.WriteAsync(readRequest);
        return await readRequest.Responses.Reader.ReadAsync();
    }

    public async Task WriteAsync(InteractorCoroutines.Token token)
    {
        await requestWriter.WriteAsync(new Write(token));
    }
}
