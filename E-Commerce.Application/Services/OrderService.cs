using AutoMapper;
using E_Commerce.Application.DTOs.Mpesa;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Interfaces;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly MpesaService _mpesaService;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        MpesaService mpesaService,
        IMapper mapper,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _mpesaService = mpesaService;
        _mapper = mapper;
        _logger = logger;
    }

    //checkout and initiate M-Pesa STK Push
    public async Task<CheckoutResponse> CheckoutAsync(Guid userId, CheckoutRequest request)
    {
        // get user cart
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);

        if (cart == null || !cart.Items.Any())
        {
            throw new InvalidOperationException("Cart is empty");
        }

        //check stock availability for each item
        foreach (var cartItem in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);

            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException(
                    $"Product {cartItem.Product.Name} is no longer available");
            }

            if (product.StockQuantity < cartItem.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for {product.Name}. Only {product.StockQuantity} available");
            }
        }

        //Calc the total
        var totalAmount = cart.Items.Sum(i => i.Quantity * i.UnitPrice);

        //generate order number
        var orderNumber = GenerateOrderNumber();

        //create order
        var order = new Order
        {
            UserId = userId,
            OrderNumber = orderNumber,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            PhoneNumber = request.PhoneNumber,
            Items = cart.Items.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ProductImageUrl = ci.Product.ImageUrl,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                TotalPrice = ci.Quantity * ci.UnitPrice
            }).ToList()
        };

        await _orderRepository.AddAsync(order);

        //initiate Mpesa STK Push
        try
        {
            var stkResponse = await _mpesaService.InitiateStkPushAsync(
                phoneNumber: request.PhoneNumber,
                amount: 1, //totalAmount, //set to 1 for testing purposes
                accountReference: orderNumber,
                transactionDesc: $"Payment for order {orderNumber}"
            );

            //update order with M-Pesa details
            order.MpesaCheckoutRequestId = stkResponse.CheckoutRequestID;
            order.MpesaMerchantRequestId = stkResponse.MerchantRequestID;
            order.Status = OrderStatus.Processing;

            await _orderRepository.UpdateAsync(order);

            //clear cart after successful STK payment
            await _cartRepository.ClearCartAsync(cart.Id);

            return new CheckoutResponse
            {
                OrderId = order.Id,
                OrderNumber = orderNumber,
                TotalAmount = totalAmount,
                Message = "Please check your phone and enter M-Pesa PIN to complete payment",
                CheckoutRequestId = stkResponse.CheckoutRequestID
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initiate M-Pesa payment for order {OrderNumber}", orderNumber);

            //update order status to failed
            order.Status = OrderStatus.Failed;
            await _orderRepository.UpdateAsync(order);

            throw new InvalidOperationException("Failed to initiate payment. Please try again.");
        }
    }

    //process mpesa callback
    public async Task ProcessMpesaCallbackAsync(MpesaCallbackRequest callback)
    {
        try
        {
            var stkCallback = callback.Body.StkCallback;
            var checkoutRequestId = stkCallback.CheckoutRequestID;

            _logger.LogInformation(
                "Processing M-Pesa callback for CheckoutRequestID: {CheckoutRequestId}, ResultCode: {ResultCode}",
                checkoutRequestId,
                stkCallback.ResultCode);

            //find the order
            var order = await _orderRepository.GetOrderByCheckoutRequestIdAsync(checkoutRequestId);

            if (order == null)
            {
                _logger.LogWarning("Order not found for CheckoutRequestID: {CheckoutRequestId}", checkoutRequestId);
                return;
            }

            //check result code
            if (stkCallback.ResultCode == 0)
            {
                //payment received
                _logger.LogInformation("Payment successful for order {OrderNumber}", order.OrderNumber);

                //get payment details from callback
                var metadata = stkCallback.CallbackMetadata?.Item;

                if (metadata != null)
                {
                    var receiptNumber = metadata
                        .FirstOrDefault(i => i.Name == "MpesaReceiptNumber")?.Value?.ToString();

                    var transactionDateStr = metadata
                        .FirstOrDefault(i => i.Name == "TransactionDate")?.Value?.ToString();

                    order.MpesaReceiptNumber = receiptNumber;
                    order.PaymentDate = ParseMpesaTransactionDate(transactionDateStr);
                }

                order.Status = OrderStatus.Completed;

                //reduce stock quantities
                foreach (var orderItem in order.Items)
                {
                    var product = await _productRepository.GetByIdAsync(orderItem.ProductId);

                    if (product != null)
                    {
                        product.StockQuantity -= orderItem.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }
            }
            else
            {
                //payment failed
                _logger.LogWarning(
                    "Payment failed for order {OrderNumber}. ResultCode: {ResultCode}, ResultDesc: {ResultDesc}",
                    order.OrderNumber,
                    stkCallback.ResultCode,
                    stkCallback.ResultDesc);

                order.Status = OrderStatus.Failed;
            }

            await _orderRepository.UpdateAsync(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing M-Pesa callback");
            throw;
        }
    }

    //get user orders
    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId)
    {
        var orders = await _orderRepository.GetUserOrdersAsync(userId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    //get order by id
    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId, Guid userId)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(orderId);

        if (order == null)
        {
            return null;
        }

        //user can only view their own orders
        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to view this order");
        }

        return _mapper.Map<OrderDto>(order);
    }

    //generate order number
    private string GenerateOrderNumber()
    {
        //date formatting
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = new Random().Next(10000, 99999);
        return $"ORD-{datePart}-{randomPart}";
    }

    //helper to parse M-Pesa transaction date
    private DateTime? ParseMpesaTransactionDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr))
            return null;

        try
        {
            //mpesa format
            var year = int.Parse(dateStr.Substring(0, 4));
            var month = int.Parse(dateStr.Substring(4, 2));
            var day = int.Parse(dateStr.Substring(6, 2));
            var hour = int.Parse(dateStr.Substring(8, 2));
            var minute = int.Parse(dateStr.Substring(10, 2));
            var second = int.Parse(dateStr.Substring(12, 2));

            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        }
        catch
        {
            return null;
        }
    }
}