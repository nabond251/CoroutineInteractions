namespace CoroutineInteractions;

public interface IInteractorInstructionWriter
{
    ValueTask WriteAsync(YieldDecompressor yieldDecompressor);

    ValueTask WriteAsync(YieldParser yieldParser);

    ValueTask WriteAsync(Read read);

    ValueTask WriteAsync(Write write);
}
