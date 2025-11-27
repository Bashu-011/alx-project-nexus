using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrastructure.Repositories;


//Implements the interface

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context)
    {
    }

    //user active carts
    public async Task<Cart?> GetActiveCartByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(c => c.Items)              //load cart items
                .ThenInclude(i => i.Product)    //load product details for each item
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);
    }

    //get cart with all details
    public async Task<Cart?> GetCartWithItemsAsync(Guid cartId)
    {
        return await _dbSet
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Category)  //load category
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }

    //clear all items
    public async Task ClearCartAsync(Guid cartId)
    {
        var cart = await _dbSet
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        if (cart != null)
        {
            cart.Items.Clear();  
            await _context.SaveChangesAsync();
        }
    }
}