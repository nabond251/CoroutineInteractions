namespace CoroutineInteractions;

public interface IInteractor
{
    Task<int> ReadAsync();

    Task WriteAsync(InteractorCoroutines.Token token);
}
