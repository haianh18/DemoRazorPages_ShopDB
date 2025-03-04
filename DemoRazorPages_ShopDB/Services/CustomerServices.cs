using DemoRazorPages_ShopDB.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoRazorPages_ShopDB.Services
{
    public class CustomerServices
    {
        ShopDbrazorPagesContext _context;

        public CustomerServices(ShopDbrazorPagesContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
    }
}
