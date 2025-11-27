using E_Commerce.Core.Entities;

namespace E_Commerce.Core.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    //get order by id including order items
    Task<Order?> GetOrderWithItemsAsync(Guid orderId);

    //get all orders for a user
    Task<IEnumerable<Order>> GetUserOrdersAsync(Guid userId);

    //find order by M-Pesa checkout request id
    Task<Order?> GetOrderByCheckoutRequestIdAsync(string checkoutRequestId);

    //get order by order number
    Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
}