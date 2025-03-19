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
            Categories = _categoryServices.GetCategories();

            Products = _productServices.GetFilteredProducts(Filter);

            // Lấy danh sách giỏ hàng để người dùng chọn
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

        public async Task<IActionResult> OnPostSelectCartAsync(int? selectedCartId)
        {
            // Nếu không chọn giỏ hàng, tạo giỏ hàng mới
            if (!selectedCartId.HasValue)
            {
                var newCart = await _cartServices.CreateCartAsync();
                selectedCartId = newCart.CartId;
            }

            // Quay lại trang với cartId được chọn
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
                // Nếu chưa chọn cart, tạo mới
                if (!SelectedCartId.HasValue)
                {
                    var newCart = await _cartServices.CreateCartAsync();
                    SelectedCartId = newCart.CartId;
                }

                // Thêm sản phẩm vào giỏ hàng
                await _cartServices.AddItemToCartAsync(SelectedCartId.Value, productId, quantity);

                TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";

                // Quay lại trang danh sách sản phẩm với các filter và trang hiện tại
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
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
        }

        public async Task<IActionResult> OnPostDeleteAsync(int productId, int? categoryid, decimal? minprice, decimal? maxprice, string searchterm)
        {
            bool success = _productServices.DeleteProduct(productId);

            // Preserve filter values
            Filter.CategoryId = categoryid;
            Filter.MinPrice = minprice;
            Filter.MaxPrice = maxprice;
            Filter.SearchTerm = searchterm;

            return RedirectToPage(new
            {
                SelectedCartId,
                categoryid = Filter.CategoryId,
                minprice = Filter.MinPrice,
                maxprice = Filter.MaxPrice,
                searchterm = Filter.SearchTerm
            });
        }
    }
}