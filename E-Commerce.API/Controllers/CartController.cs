using E_Commerce.Application.DTOs.Cart;
using E_Commerce.Application.DTOs.Common;
using E_Commerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  //require authentication
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    //helper method to get user_id from teh token
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        return userId;
    }

    //get users cart
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(ApiResponse<CartDto>.SuccessResponse(cart));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart");
            return StatusCode(500, ApiResponse<CartDto>.ErrorResponse("Error retrieving cart"));
        }
    }

    //Add item to cart
    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CartDto>>> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.AddToCartAsync(userId, request);
            return Ok(ApiResponse<CartDto>.SuccessResponse(cart, "Item added to cart"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to add item to cart");
            return BadRequest(ApiResponse<CartDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
            return StatusCode(500, ApiResponse<CartDto>.ErrorResponse("Error adding item to cart"));
        }
    }

    //update cart item qtty
    [HttpPut("items/{cartItemId}")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CartDto>>> UpdateCartItem(
        Guid cartItemId,
        [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, cartItemId, request);
            return Ok(ApiResponse<CartDto>.SuccessResponse(cart, "Cart updated"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update cart item {CartItemId}", cartItemId);
            return BadRequest(ApiResponse<CartDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item {CartItemId}", cartItemId);
            return StatusCode(500, ApiResponse<CartDto>.ErrorResponse("Error updating cart"));
        }
    }

    //remove item from cart
    [HttpDelete("items/{cartItemId}")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CartDto>>> RemoveFromCart(Guid cartItemId)
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            return Ok(ApiResponse<CartDto>.SuccessResponse(cart, "Item removed from cart"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to remove cart item {CartItemId}", cartItemId);
            return NotFound(ApiResponse<CartDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
            return StatusCode(500, ApiResponse<CartDto>.ErrorResponse("Error removing item"));
        }
    }

    //clear cart
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> ClearCart()
    {
        try
        {
            var userId = GetUserId();
            var result = await _cartService.ClearCartAsync(userId);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Cart cleared"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Error clearing cart"));
        }
    }
}