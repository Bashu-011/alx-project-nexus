using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTOs.Cart;
public class CartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public DateTime LastModified { get; set; }

    //Calculated properties
    public int TotalItems => Items.Sum(i => i.Quantity);
    public decimal SubTotal => Items.Sum(i => i.TotalPrice);
}
