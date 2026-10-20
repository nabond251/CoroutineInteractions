using System.Threading.Channels;

namespace CoroutineInteractions;

public record class Correspondance<TRequest, TResponse>(
    TRequest Request,
    Channel<TResponse> Responses);
