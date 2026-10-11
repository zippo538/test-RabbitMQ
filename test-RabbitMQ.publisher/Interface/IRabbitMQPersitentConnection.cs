using RabbitMQ.Client;

namespace test_RabbitMQ.Interface;

public interface IRabbitMqPersitentConnection : IDisposable
{
    bool IsConnected { get; }
    Task<bool> TryConnectAsync(CancellationToken cancellationToken = default);
    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
    
}