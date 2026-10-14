using Microsoft.AspNetCore.Mvc;
using test_RabbitMQ.Dto;
using test_RabbitMQ.Service.Interface;

namespace test_RabbitMQ.Controllers;
[ApiController]
[Route("api/[Controller]")]
public class MessagesController (IMessageProducer messageProducer) : ControllerBase
{
    private readonly IMessageProducer _messageProducer = messageProducer;
    
    [HttpPost("publish")]
    public async Task<IActionResult> PublishMessage([FromBody]OrderRequestDto requestDto)
    {
        if (requestDto == null || string.IsNullOrWhiteSpace(requestDto.ItemName))
        {
            return BadRequest("Data pesanan tidak valid");
        }
        
        string queueName = "order";

        await _messageProducer.SendMessageAsync(requestDto,queueName);
        return Accepted(new { 
            Status = "Pesan berhasil dikirim ke RabbitMQ!", 
            Timestamp = DateTime.UtcNow,
            Data = requestDto 
        });

    }
}
