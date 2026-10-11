using System.Net.Sockets;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using test_RabbitMQ.Interface;

namespace test_RabbitMQ.Infrastructure;

public class RabbitMQPersistentConnection(ConnectionFactory connectionFactory) : IRabbitMqPersitentConnection
{
    private readonly ConnectionFactory _connectionFactory = connectionFactory;
    private IConnection? _connection;
    private readonly object _lock = new();
    private bool _disposed;

    public RabbitMQPersistentConnection() : this(new ConnectionFactory{ HostName = "locathost" }) {}
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _connection?.Dispose();
    }

    public bool IsConnected => _connection is {IsOpen:true} && !_disposed;
    
    public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (IsConnected) return true;
        }
        var policy = Policy.Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) => Console.WriteLine($"RabbitMQ client could not connect after {time.TotalSeconds:n1}s: {ex.Message}"));

        await policy.ExecuteAsync(async () =>
        {
            _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        });

        return IsConnected;
    }

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            await TryConnectAsync(cancellationToken);
        }
        if (!IsConnected)
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        return await _connection!.CreateChannelAsync(cancellationToken: cancellationToken);
    }
}