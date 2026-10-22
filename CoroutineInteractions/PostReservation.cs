using CoroutineInteractions.ReservationsApi;
using System.Threading.Channels;

namespace CoroutineInteractions;

public record class PostReservation(Reservation Request) :
    Correspondance<Reservation, object?>(
        Request,
        Channel.CreateBounded<object?>(new BoundedChannelOptions(0)));
