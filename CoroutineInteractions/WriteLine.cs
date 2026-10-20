using System.Threading.Channels;

namespace CoroutineInteractions;

public record class WriteLine(string Request) :
    Correspondance<string, object?>(
        Request,
        Channel.CreateBounded<object?>(new BoundedChannelOptions(0)));
