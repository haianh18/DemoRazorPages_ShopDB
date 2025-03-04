using DemoRazorPages_ShopDB.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoRazorPages_ShopDB.Services
{
    public class EmployeeServices
    {
        ShopDbrazorPagesContext _context;

        public EmployeeServices(ShopDbrazorPagesContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }
    }
}
