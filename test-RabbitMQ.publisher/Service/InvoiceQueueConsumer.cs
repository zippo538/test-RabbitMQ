using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using test_RabbitMQ.Dto;

namespace test_RabbitMQ.Service;

public class InvoiceQueueConsumer : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory {HostName = "localhost"};
        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(queue:"invoice_jobs", durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        
        var consummer = new AsyncEventingBasicConsumer(_channel);
        consummer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonString = Encoding.UTF8.GetString(body);
            var invoice = JsonSerializer.Deserialize<InvoiceDto>(jsonString);
            Console.WriteLine($"[API Background Worker] Processed Invoice ID: {invoice?.Id}");
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };
        await _channel.BasicConsumeAsync(queue: "invoice_jobs", autoAck: true, consumer: consummer);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.CloseAsync(cancellationToken);
        if (_connection is not null) await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}