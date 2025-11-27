using E_Commerce.Application.DTOs.Common;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    //dependencies
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;


    //constructor
    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    //get user id from token
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        return userId;
    }

    //checkout and initiate mpesa payment
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(ApiResponse<CheckoutResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CheckoutResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CheckoutResponse>>> Checkout([FromBody] CheckoutRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.CheckoutAsync(userId, request);
            return Ok(ApiResponse<CheckoutResponse>.SuccessResponse(result, "Payment initiated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Checkout failed for user");
            return BadRequest(ApiResponse<CheckoutResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during checkout");
            return StatusCode(500, ApiResponse<CheckoutResponse>.ErrorResponse("An error occurred during checkout"));
        }
    }

    

    //get user order history
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders()
    {
        try
        {
            var userId = GetUserId();
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResponse("Error retrieving orders"));
        }
    }


    //get specific order details
    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(Guid orderId)
    {
        try
        {
            var userId = GetUserId();
            var order = await _orderService.GetOrderByIdAsync(orderId, userId);

            if (order == null)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found"));
            }

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("Error retrieving order"));
        }
    }
}