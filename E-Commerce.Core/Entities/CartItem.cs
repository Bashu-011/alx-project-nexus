using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Entities;

//properties for the items in the cart
public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    
    public decimal UnitPrice { get; set; }

    public Cart Cart { get; set; } = null!;

    //Foreign key
    public Product Product { get; set; } = null!;
}
