namespace CoroutineInteractions;

public interface ICommandLine
{
    Task<string?> ReadLineAsync();

    Task WriteLineAsync(string text);
}
