using System.Threading.Channels;

namespace CoroutineInteractions;

public class InteractorInstructionWriter(
    ChannelWriter<InteractorInstruction> channelWriter) :
    IInteractorInstructionWriter
{
    public async ValueTask WriteAsync(Read read) =>
        await channelWriter.WriteAsync(read);

    public async ValueTask WriteAsync(Write write) =>
        await channelWriter.WriteAsync(write);
}
