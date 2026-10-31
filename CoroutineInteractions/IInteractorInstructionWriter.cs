namespace CoroutineInteractions;

public interface IInteractorInstructionWriter
{
    ValueTask WriteAsync(Read read);

    ValueTask WriteAsync(Write write);
}
