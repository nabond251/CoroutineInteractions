using CoroutineInteractions.ReservationsApi;

namespace CoroutineInteractions;

public class CoroutineReservationsApi(
    IReservationsApiInstructionWriter requestWriter) : IReservationsApi
{
    public async Task<IEnumerable<Slot>> GetSlotsAsync(DateTimeOffset slotDate)
    {
        var getSlotsRequest = new GetSlots(slotDate);
        await requestWriter.WriteAsync(getSlotsRequest);
        return await getSlotsRequest.Responses.Reader.ReadAsync();
    }

    public async Task PostReservationAsync(Reservation reservation)
    {
        await requestWriter.WriteAsync(new PostReservation(reservation));
    }
}
