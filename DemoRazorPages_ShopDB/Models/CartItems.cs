using System;
using System.Collections.Generic;

namespace DemoRazorPages_ShopDB.Models;

public class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; } = 1;

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}