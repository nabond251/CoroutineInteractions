using CoroutineInteractions.ReservationsApi;

namespace CoroutineInteractions;

public interface IReservationsApi
{
    Task<IEnumerable<Slot>> GetSlotsAsync(DateTimeOffset slotDate);

    Task PostReservationAsync(Reservation reservation);
}
