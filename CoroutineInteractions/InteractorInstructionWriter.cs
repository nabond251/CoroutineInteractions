using System.Threading.Channels;

namespace CoroutineInteractions;

public class InteractorInstructionWriter(
    ChannelWriter<InteractorInstruction> channelWriter) :
    IInteractorInstructionWriter
{
    public async ValueTask WriteAsync(YieldDecompressor yieldDecompressor) =>
        await channelWriter.WriteAsync(yieldDecompressor);

    public async ValueTask WriteAsync(YieldParser yieldParser) =>
        await channelWriter.WriteAsync(yieldParser);

    public async ValueTask WriteAsync(Read read) =>
        await channelWriter.WriteAsync(read);

    public async ValueTask WriteAsync(Write write) =>
        await channelWriter.WriteAsync(write);
}
