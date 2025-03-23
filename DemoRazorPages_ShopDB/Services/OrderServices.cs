using DemoRazorPages_ShopDB.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoRazorPages_ShopDB.Services
{
    public class OrderServices
    {
        ShopDbrazorPagesContext _context;

        public OrderServices(ShopDbrazorPagesContext context)
        {
            _context = context;
        }

        public async Task<List<Order>?> GetAllOrderAsync(OrderFilterModel? filter)
        {
            if (filter == null) return null;
            IQueryable<Order> result = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee);

            if (filter?.EmployeeId != null && filter.EmployeeId != 0) result = result.Where(o => o.EmployeeId == filter.EmployeeId);
            if (filter?.CustomerId != null && filter.CustomerId != 0) result = result.Where(o => o.CustomerId.Equals(filter.CustomerId));
            if (filter?.ProductId != null) result = result.Where(o => o.OrderDetails.Any(d => d.ProductId == filter.ProductId));
            if (filter?.FromDate != null) result = result.Where(o => o.OrderDate >= filter.FromDate);
            if (filter?.ToDate != null) result = result.Where(o => o.OrderDate <= filter.ToDate);
            return await result.ToListAsync();
        }

        public async Task<Order?> GetOrderAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> Delete(int OrderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == OrderId);
            if (order == null) return false;
            var details = _context.OrderDetails.Where(o => o.OrderId == OrderId);
            if (details.Any()) _context.OrderDetails.RemoveRange(details);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetOrderCountAsync()
        {
            return await _context.Orders.CountAsync();
        }
    }
}
