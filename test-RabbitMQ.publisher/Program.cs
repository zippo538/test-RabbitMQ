using test_RabbitMQ.Infrastructure;
using test_RabbitMQ.Interface;
using test_RabbitMQ.Service;
using test_RabbitMQ.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
builder.Services.AddSingleton<IRabbitMqPersitentConnection, RabbitMQPersistentConnection>();


builder.Services.AddHostedService<InvoiceQueueConsumer>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();


app.Run();

