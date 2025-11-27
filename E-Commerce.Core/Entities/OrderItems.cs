namespace E_Commerce.Core.Entities;

public class OrderItem : BaseEntity
{
    //which order is item attached
    public Guid OrderId { get; set; }

   
    public Guid ProductId { get; set; }

    //snapshot at time of purchse
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;

    //qtty
    public int Quantity { get; set; }

    //price ar time of purchase
    public decimal UnitPrice { get; set; }

    //CALCULATED
    public decimal TotalPrice { get; set; }

    //foreign keys
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}