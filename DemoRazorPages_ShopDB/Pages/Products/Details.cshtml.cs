using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoRazorPages_ShopDB.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly CartServices _cartServices;
        private readonly ProductServices _productServices;
        private readonly ShopDbrazorPagesContext _context;

        public DetailsModel(ProductServices productServices, ShopDbrazorPagesContext context, CartServices cartServices)
        {
            _productServices = productServices;
            _context = context;
            _cartServices = cartServices;
        }
        public List<Cart> Carts { get; set; } = new List<Cart>();
        public Product Product { get; set; } = default!;
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = _productServices.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            Product = product;

            // Lấy danh sách giỏ hàng cho dropdown
            Carts = await _cartServices.GetAllCartsAsync();

            // Nếu không có giỏ hàng nào, tạo một giỏ hàng mặc định
            if (!Carts.Any())
            {
                var newCart = await _cartServices.CreateCartAsync();
                Carts.Add(newCart);
            }

            // Lấy thông tin chi tiết đơn hàng có sản phẩm này
            OrderDetails = await _context.OrderDetails
                .Where(od => od.ProductId == id)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .OrderByDescending(od => od.Order.OrderDate)
                .ToListAsync();

            return Page();
        }
    }
}