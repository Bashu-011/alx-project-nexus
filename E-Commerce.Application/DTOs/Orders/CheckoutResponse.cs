namespace E_Commerce.Application.DTOs.Orders;

public class CheckoutResponse
{
	public Guid OrderId { get; set; }
	public string OrderNumber { get; set; } = string.Empty;
	public decimal TotalAmount { get; set; }
	public string Message { get; set; } = string.Empty;
	public string CheckoutRequestId { get; set; } = string.Empty;
}