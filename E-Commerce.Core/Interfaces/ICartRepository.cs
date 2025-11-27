using E_Commerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        //get user active carts
        Task<Cart?> GetActiveCartByUserIdAsync(Guid userId);

        //get cart items
        Task<Cart?> GetCartWithItemsAsync(Guid cartId);

        //clear items
        Task ClearCartAsync(Guid cartId);
    }
}
