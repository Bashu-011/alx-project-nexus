using AutoMapper;
using E_Commerce.Application.DTOs.Cart;
using E_Commerce.Application.Interfaces;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;

namespace E_Commerce.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    //get cart

    public async Task<CartDto> GetCartAsync(Guid userId)
    {
        //users active cart
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);


        //if no cart exists, create a new empty one
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                IsActive = true,
                LastModified = DateTime.UtcNow
            };

            await _cartRepository.AddAsync(cart);
        }

        //convert to dto and return
        return _mapper.Map<CartDto>(cart);
    }

    //add to cart

    public async Task<CartDto> AddToCartAsync(Guid userId, AddToCartRequest request)
    {
        //validate if product exists and is available
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        if (!product.IsActive)
        {
            throw new InvalidOperationException("Product is not available");
        }

        if (product.StockQuantity < request.Quantity)
        {
            throw new InvalidOperationException(
                $"Only {product.StockQuantity} units available in stock");
        }

        //get / create cart
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                IsActive = true,
                LastModified = DateTime.UtcNow
            };

            await _cartRepository.AddAsync(cart);
            //reload
            cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);
        }

        //check if product already in cart
        var existingItem = cart!.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

        if (existingItem != null)
        {
            //increase quantity if prd in cart
            existingItem.Quantity += request.Quantity;



            //check stock again after increases
            if (existingItem.Quantity > product.StockQuantity)
            {
                throw new InvalidOperationException(
                    $"Cannot add {request.Quantity} more. Only {product.StockQuantity} units available");
            }

            await _cartRepository.UpdateAsync(cart);
        }
        else
        {
            //new prd - add to cart
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price  //snapshot of price
            };

            cart.Items.Add(cartItem);
            cart.LastModified = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(cart);
        }

        //reload cart with updated items and return
        cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);
        return _mapper.Map<CartDto>(cart);
    }

    //update cart item

    public async Task<CartDto> UpdateCartItemAsync(
        Guid userId,
        Guid cartItemId,
        UpdateCartItemRequest request)
    {
        //get users cart
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);

        if (cart == null)
        {
            throw new InvalidOperationException("Cart not found");
        }

        //find the cart item
        var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (cartItem == null)
        {
            throw new InvalidOperationException("Item not found in cart");
        }

        //validate new quantity
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be greater than 0");
        }

        //check stock availability
        var product = await _productRepository.GetByIdAsync(cartItem.ProductId);

        if (product == null || !product.IsActive)
        {
            throw new InvalidOperationException("Product is no longer available");
        }

        if (request.Quantity > product.StockQuantity)
        {
            throw new InvalidOperationException(
                $"Only {product.StockQuantity} units available in stock");
        }

        // update quantity
        cartItem.Quantity = request.Quantity;
        cart.LastModified = DateTime.UtcNow;

        await _cartRepository.UpdateAsync(cart);

        //reload and return
        cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);
        return _mapper.Map<CartDto>(cart);
    }

    //remove from cart
    public async Task<CartDto> RemoveFromCartAsync(Guid userId, Guid cartItemId)
    {
        //get user's cart
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);

        if (cart == null)
        {
            throw new InvalidOperationException("Cart not found");
        }

        //find and remove the item
        var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (cartItem == null)
        {
            throw new InvalidOperationException("Item not found in cart");
        }

        cart.Items.Remove(cartItem);
        cart.LastModified = DateTime.UtcNow;

        await _cartRepository.UpdateAsync(cart);

        //reload and return
        cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);
        return _mapper.Map<CartDto>(cart);
    }

    //remove all items from cart
    public async Task<bool> ClearCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);

        if (cart == null)
        {
            return false;
        }

        await _cartRepository.ClearCartAsync(cart.Id);
        return true;
    }
}