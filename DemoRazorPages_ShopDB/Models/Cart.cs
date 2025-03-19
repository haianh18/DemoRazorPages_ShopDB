using System;
using System.Collections.Generic;

namespace DemoRazorPages_ShopDB.Models;

public class Cart
{
    public int CartId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}