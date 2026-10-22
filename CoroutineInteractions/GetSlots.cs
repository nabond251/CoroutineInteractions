using CoroutineInteractions.ReservationsApi;
using System.Threading.Channels;

namespace CoroutineInteractions;

public record class GetSlots() :
    Correspondance<object?, ICollection<Slot>>(
        null,
        Channel.CreateUnbounded<ICollection<Slot>>());
