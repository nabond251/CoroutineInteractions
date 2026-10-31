using System.Threading.Channels;

namespace CoroutineInteractions;

public record class Read() :
    Correspondance<object?, int>(
        null,
        Channel.CreateUnbounded<int>());
