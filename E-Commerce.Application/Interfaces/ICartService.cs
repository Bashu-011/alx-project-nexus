using E_Commerce.Application.DTOs.Cart;

namespace E_Commerce.Application.Interfaces;

public interface ICartService
{
    //users curret cart
    Task<CartDto> GetCartAsync(Guid userId);

    //add item to cart
    Task<CartDto> AddToCartAsync(Guid userId, AddToCartRequest request);

    //add/minus quantity of item in cart
    Task<CartDto> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemRequest request);

    //remove item from cart
    Task<CartDto> RemoveFromCartAsync(Guid userId, Guid cartItemId);

    //Clear cart
    Task<bool> ClearCartAsync(Guid userId);
}