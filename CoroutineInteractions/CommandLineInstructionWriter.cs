using System.Threading.Channels;

namespace CoroutineInteractions;

public class CommandLineInstructionWriter(
    ChannelWriter<CommandLineInstruction> channelWriter) :
    ICommandLineInstructionWriter
{
    public async ValueTask WriteAsync(ReadLine readLine) =>
        await channelWriter.WriteAsync(readLine);

    public async ValueTask WriteAsync(WriteLine writeLine) =>
        await channelWriter.WriteAsync(writeLine);
}
