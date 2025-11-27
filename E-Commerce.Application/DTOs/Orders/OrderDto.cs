namespace E_Commerce.Application.DTOs.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? MpesaReceiptNumber { get; set; }
    public DateTime? PaymentDate { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}