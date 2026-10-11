using Microsoft.AspNetCore.Mvc;
using test_RabbitMQ.Dto;
using test_RabbitMQ.Service.Interface;

namespace test_RabbitMQ.Controllers;
[ApiController]
[Route("api/[controller]")]
public class InvoicesController(IMessageProducer producer) : ControllerBase
{
    private readonly IMessageProducer _producer = producer;

    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] InvoiceDto invoice)
    {
        await _producer.SendMessageAsync(invoice, "invoice jobs");
        return Accepted(new
        {
            status = "success",
        });
    }
}