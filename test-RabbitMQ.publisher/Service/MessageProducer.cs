using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using test_RabbitMQ.Interface;
using test_RabbitMQ.Service.Interface;

namespace test_RabbitMQ.Service;

public class MessageProducer(IRabbitMqPersitentConnection connection) : IMessageProducer
{
    private readonly IRabbitMqPersitentConnection _connection = connection;
  
    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        await using var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        
        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);
        await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);
    }
}