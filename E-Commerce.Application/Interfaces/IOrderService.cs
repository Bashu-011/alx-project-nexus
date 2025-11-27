using E_Commerce.Application.DTOs.Orders;

namespace E_Commerce.Application.Interfaces;

public interface IOrderService
{
    //start checkout and stk push
    Task<CheckoutResponse> CheckoutAsync(Guid userId, CheckoutRequest request);

    //get user order history
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId);

    //specific order details
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId, Guid userId);

    //process mpesa callback
    Task ProcessMpesaCallbackAsync(MpesaCallbackRequest callback);
}