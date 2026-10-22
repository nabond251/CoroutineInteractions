using System.Threading.Channels;

namespace CoroutineInteractions;

public class WizardInstructionWriter(
    ChannelWriter<WizardInstruction> channelWriter) : IWizardInstructionWriter
{
    public async ValueTask WriteAsync(ReadLine readLine) =>
        await channelWriter.WriteAsync((CommandLineInstruction)readLine);

    public async ValueTask WriteAsync(WriteLine writeLine) =>
        await channelWriter.WriteAsync((CommandLineInstruction)writeLine);

    public async ValueTask WriteAsync(GetSlots getSlots) =>
        await channelWriter.WriteAsync((ReservationsApiInstruction)getSlots);

    public async ValueTask WriteAsync(PostReservation postReservation) =>
        await channelWriter.WriteAsync((ReservationsApiInstruction)postReservation);
}
