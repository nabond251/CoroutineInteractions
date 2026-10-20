using System.Threading.Channels;

namespace CoroutineInteractions;

public record class ReadLine(
    ChannelReader<string> ResponseReader) :
    Correspondance<object?, string>(null, ResponseReader);
