using CoroutineInteractions;
using CoroutineInteractions.ReservationsApi;
using System.Threading.Channels;

var requestChannel = Channel.CreateUnbounded<WizardInstruction>();

var coroutine = Task.Run(async () =>
{
    await WizardCoroutines.TryReserveAsync(
        new WizardInstructionWriter(requestChannel.Writer));
    requestChannel.Writer.Complete();
});

var httpClient = new HttpClient();
var apiClient = new ReservationsApiClient(httpClient);
await foreach (var instruction in requestChannel.Reader.ReadAllAsync())
{
    instruction.Switch(
        commandLineInstruction => commandLineInstruction.Switch(
            readLine =>
            {
                var response = Console.ReadLine();
                var writer = readLine.Responses.Writer;
                writer.TryWrite(response);
                writer.Complete();
            },
            writeLine =>
            {
                var request = writeLine.Request;
                Console.WriteLine(request);
            }),
        reservationsApiInstruction => reservationsApiInstruction.Switch(
            async getSlots =>
            {
                var response = await apiClient.GetSlotsAsync(getSlots.Request);
                var writer = getSlots.Responses.Writer;
                await writer.WriteAsync(response);
                writer.Complete();
            },
            async postReservation =>
            {
                var request = postReservation.Request;
                await apiClient.PostReservationAsync(request);
            }));
}

await coroutine;
