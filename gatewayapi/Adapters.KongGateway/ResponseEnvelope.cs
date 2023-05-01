namespace Adapters.KongGateway;

public class ResponseEnvelope<T>
{
    public List<T> Data { get; set; } = new();
}

