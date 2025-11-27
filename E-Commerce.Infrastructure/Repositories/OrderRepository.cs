using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetOrderWithItemsAsync(Guid orderId)
    {
        return await _dbSet
            .Include(o => o.Items)           //load order items
                .ThenInclude(i => i.Product) //load product for each item
            .Include(o => o.User)            //load user info
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(Guid userId)
    {
        return await _dbSet
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)  //newest first
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByCheckoutRequestIdAsync(string checkoutRequestId)
    {
        return await _dbSet
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.MpesaCheckoutRequestId == checkoutRequestId);
    }

    public async Task<Order?> GetOrderByOrderNumberAsync(string orderNumber)
    {
        return await _dbSet
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }
}