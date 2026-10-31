namespace CoroutineInteractions;

public interface IInteractor
{
    Task<int> YieldParserAsync();

    Task YieldDecompressorAsync(int c);

    Task<int> ReadAsync();

    Task WriteAsync(InteractorCoroutines.Token token);
}
