using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Entities;

//Property for the cart

public class Cart : BaseEntity
{
    public Guid UserId { get; set; }

    public bool IsActive { get; set; } = true;

   
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    //foreign key
    public User User { get; set; } = null!;

    //list of items in cart
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
