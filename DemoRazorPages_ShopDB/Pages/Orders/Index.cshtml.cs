using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DemoRazorPages_ShopDB.Models;

namespace DemoRazorPages_ShopDB.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly DemoRazorPages_ShopDB.Models.ShopDbrazorPagesContext _context;

        public IndexModel(DemoRazorPages_ShopDB.Models.ShopDbrazorPagesContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee).ToListAsync();
        }
    }
}
