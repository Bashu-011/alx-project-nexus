namespace E_Commerce.Core.Entities;

public class Order : BaseEntity
{
	//user
	public Guid UserId { get; set; }

	public string OrderNumber { get; set; } = string.Empty;

	public decimal TotalAmount { get; set; }

	public OrderStatus Status { get; set; } = OrderStatus.Pending;

	public string? PhoneNumber { get; set; }
	public string? MpesaReceiptNumber { get; set; }
	public DateTime? PaymentDate { get; set; }

	public string? MpesaCheckoutRequestId { get; set; }
	public string? MpesaMerchantRequestId { get; set; }

	public User User { get; set; } = null!;
	public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
	Pending = 0,        //order created, awaiting payment
	Processing = 1,     //payment initiated (STK)
	Completed = 2,      //payment successful
	Failed = 3,         //payment failed
	Cancelled = 4       //user cancelled
}