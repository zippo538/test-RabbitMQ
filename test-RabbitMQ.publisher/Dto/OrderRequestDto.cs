using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test_RabbitMQ.Dto;

public record OrderRequestDto
{
    public int OrderId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
