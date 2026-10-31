using System.Threading.Channels;

namespace CoroutineInteractions;

public record class YieldDecompressor(int Request) :
    Correspondance<int, object?>(
        Request,
        Channel.CreateBounded<object?>(new BoundedChannelOptions(0)));
