namespace CoroutineInteractions;

public interface IReservationsApiInstructionWriter
{
    ValueTask WriteAsync(GetSlots getSlots);

    ValueTask WriteAsync(PostReservation postReservation);
}
