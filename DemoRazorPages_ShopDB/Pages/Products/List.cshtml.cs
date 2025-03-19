using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Pages.Shared;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Products
{
    public class ListModel : PageModel
    {
        private readonly ProductServices _productServices;
        private readonly CategoryServices _categoryServices;
        private readonly CartServices _cartServices;

        public ListModel(
            ProductServices productServices,
            CategoryServices categoryServices,
            CartServices cartServices)
        {
            _productServices = productServices;
            _categoryServices = categoryServices;
            _cartServices = cartServices;
        }

        [BindProperty(SupportsGet = true)]
        public ProductFilterModel Filter { get; set; } = new ProductFilterModel();

        [BindProperty(SupportsGet = true)]
        public int? pageIndex { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedCartId { get; set; }

        public List<Product>? Products { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Cart>? Carts { get; set; }

        public PaginationModel<Product> Pagination { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostSelectCartAsync(int? selectedCartId)
        {
            SelectedCartId = selectedCartId;

            // Redirect to the same page while preserving all filter parameters
            return RedirectToPage(new
            {
                SelectedCartId = selectedCartId,
                pageIndex = pageIndex ?? 1,
                categoryid = Filter.CategoryId,
                minprice = Filter.MinPrice,
                maxprice = Filter.MaxPrice,
                searchterm = Filter.SearchTerm
            });
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity = 1)
        {
            try
            {
                // Create a new cart only if no cart is selected and the user clicks Add To Cart
                if (!SelectedCartId.HasValue)
                {
                    var newCart = await _cartServices.CreateCartAsync();
                    SelectedCartId = newCart.CartId;
                }

                // Add product to cart
                await _cartServices.AddItemToCartAsync(SelectedCartId.Value, productId, quantity);

                TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            // Preserve all filters and parameters
            return RedirectToPage(new
            {
                SelectedCartId,
                pageIndex = pageIndex ?? 1,
                categoryid = Filter.CategoryId,
                minprice = Filter.MinPrice,
                maxprice = Filter.MaxPrice,
                searchterm = Filter.SearchTerm
            });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int productId, int? categoryid, decimal? minprice, decimal? maxprice, string searchterm)
        {
            bool success = _productServices.DeleteProduct(productId);

            if (success)
            {
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm vì nó đang được sử dụng trong đơn hàng!";
            }

            // Preserve filter values
            Filter.CategoryId = categoryid;
            Filter.MinPrice = minprice;
            Filter.MaxPrice = maxprice;
            Filter.SearchTerm = searchterm;

            return RedirectToPage(new
            {
                SelectedCartId,
                pageIndex = pageIndex ?? 1,
                categoryid = Filter.CategoryId,
                minprice = Filter.MinPrice,
                maxprice = Filter.MaxPrice,
                searchterm = Filter.SearchTerm
            });
        }

        private async Task LoadDataAsync()
        {
            Categories = _categoryServices.GetCategories();
            Products = _productServices.GetFilteredProducts(Filter);
            Carts = await _cartServices.GetAllCartsAsync();

            if (Products != null)
            {
                int pageSize = 10;
                int currentPageIndex = pageIndex ?? 1;
                int totalProducts = Products.Count;

                var products = Products
                    .Skip((currentPageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                Pagination = new PaginationModel<Product>
                {
                    PageIndex = currentPageIndex,
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                    Items = products,
                    Filter = Filter
                };
            }
        }
    }
}