using DemoRazorPages_ShopDB.Models;

namespace DemoRazorPages_ShopDB.Services
{
    public class CategoryServices
    {
        ShopDbrazorPagesContext _context;

        public CategoryServices(ShopDbrazorPagesContext context)
        {
            _context = context;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
    }
}
