namespace CoroutineInteractions;

public interface ICommandLineInstructionWriter
{
    ValueTask WriteAsync(ReadLine readLine);

    ValueTask WriteAsync(WriteLine writeLine);
}
