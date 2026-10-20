using System.Threading.Channels;

namespace CoroutineInteractions;

public record class ReadLine() :
    Correspondance<object?, string?>(
        null,
        Channel.CreateUnbounded<string?>());
