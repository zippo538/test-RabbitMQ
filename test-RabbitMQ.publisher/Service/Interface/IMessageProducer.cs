namespace test_RabbitMQ.Service.Interface;

public interface IMessageProducer
{
    Task SendMessageAsync<T>(T message, string queueName);
}