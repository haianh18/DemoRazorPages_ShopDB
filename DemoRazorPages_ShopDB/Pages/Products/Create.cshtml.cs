using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly ProductServices _productServices;
        private readonly CategoryServices _categoryServices;

        public CreateModel(ProductServices productServices, CategoryServices categoryServices)
        {
            _productServices = productServices;
            _categoryServices = categoryServices;
        }

        [BindProperty]
        public Product Product { get; set; } = new Product();

        public List<Category> Categories { get; set; } = new List<Category>();

        public void OnGet()
        {
            Categories = _categoryServices.GetCategories();
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

            bool success = _productServices.AddProduct(Product);

            if (success)
            {
                TempData["SuccessMessage"] = "Sản phẩm đã được thêm thành công!";
                return RedirectToPage("./List");
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm sản phẩm";
                Categories = _categoryServices.GetCategories();
                return Page();
            }
        }
    }
}