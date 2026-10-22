using CoroutineInteractions.ReservationsApi;
using System.Threading.Channels;

namespace CoroutineInteractions;

public record class GetSlots(DateTimeOffset slotDate) :
    Correspondance<DateTimeOffset, ICollection<Slot>>(
        slotDate,
        Channel.CreateUnbounded<ICollection<Slot>>());
