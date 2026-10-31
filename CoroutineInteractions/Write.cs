using System.Threading.Channels;

namespace CoroutineInteractions;

public record class Write(InteractorCoroutines.Token Request) :
    Correspondance<InteractorCoroutines.Token, object?>(
        Request,
        Channel.CreateBounded<object?>(new BoundedChannelOptions(0)));
