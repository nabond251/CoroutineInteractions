using System.Threading.Channels;

namespace CoroutineInteractions;

public record class Correspondance<TRequest, TResponse>(
    TRequest Request,
    ChannelReader<TResponse> ResponseReader);
