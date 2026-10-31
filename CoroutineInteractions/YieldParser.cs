using System.Threading.Channels;

namespace CoroutineInteractions;

public record class YieldParser() :
    Correspondance<object?, int>(
        null,
        Channel.CreateUnbounded<int>());
