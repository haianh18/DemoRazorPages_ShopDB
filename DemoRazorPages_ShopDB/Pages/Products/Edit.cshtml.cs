using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoRazorPages_ShopDB.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly ProductServices _productServices;
        private readonly CategoryServices _categoryServices;
        private readonly ShopDbrazorPagesContext _context;

        public EditModel(ProductServices productServices, CategoryServices categoryServices, ShopDbrazorPagesContext context)
        {
            _productServices = productServices;
            _categoryServices = categoryServices;
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public List<Category> Categories { get; set; } = new List<Category>();

        public IActionResult OnGet(int id)
        {
            var product = _productServices.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            Product = product;
            Categories = _categoryServices.GetCategories();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Categories = _categoryServices.GetCategories();
                return Page();
            }

            // Validate price
            if (Product.Price <= 0)
            {
                ModelState.AddModelError("Product.Price", "Giá phải lớn hơn 0");
                Categories = _categoryServices.GetCategories();
                return Page();
            }

            // Validate quantity
            if (Product.Quantity < 0)
            {
                ModelState.AddModelError("Product.Quantity", "Số lượng không thể âm");
                Categories = _categoryServices.GetCategories();
                return Page();
            }

            // Thêm logic kiểm tra đặc biệt nếu giảm số lượng tồn kho
            var existingProduct = _productServices.GetProduct(Product.ProductId);
            if (existingProduct != null && Product.Quantity < existingProduct.Quantity)
            {
                // Kiểm tra xem có đủ số lượng để hoàn thành các đơn hàng trong giỏ không
                // Đây là ví dụ đơn giản, bạn có thể cần kiểm tra trong tất cả các giỏ hàng
                var usedInCarts = _context.CartItems.Where(ci => ci.ProductId == Product.ProductId).Sum(ci => ci.Quantity);
                if (Product.Quantity < usedInCarts)
                {
                    ModelState.AddModelError("Product.Quantity", $"Không thể giảm số lượng xuống {Product.Quantity}. Đã có {usedInCarts} sản phẩm trong giỏ hàng của khách.");
                    Categories = _categoryServices.GetCategories();
                    return Page();
                }
            }

            bool success = _productServices.UpdateProduct(Product);

            if (success)
            {
                TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công!";
                return RedirectToPage("./Details", new { id = Product.ProductId });
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật sản phẩm";
                Categories = _categoryServices.GetCategories();
                return Page();
            }
        }
    }
}