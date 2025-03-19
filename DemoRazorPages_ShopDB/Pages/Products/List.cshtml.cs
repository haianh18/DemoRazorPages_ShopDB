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

        public ListModel(ProductServices productServices, CategoryServices categoryServices)
        {
            _productServices = productServices;
            _categoryServices = categoryServices;
        }

        [BindProperty(SupportsGet = true)]
        public ProductFilterModel Filter { get; set; } = new ProductFilterModel();

        public List<Product>? Products { get; set; }
        public List<Category>? Categories { get; set; }

        public PaginationModel<Product> Pagination { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            Categories = _categoryServices.GetCategories();
            Products = _productServices.GetFilteredProducts(Filter);

            if (Products != null)
            {
                int pageSize = 10;
                int currentPageIndex = pageIndex ?? 1;
                int totalProducts = Products.Count;

                var products = Products.Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList();

                Pagination = new PaginationModel<Product>
                {
                    PageIndex = currentPageIndex,
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                    Items = products,
                    Filter = Filter
                };
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
                categoryid = Filter.CategoryId,
                minprice = Filter.MinPrice,
                maxprice = Filter.MaxPrice,
                searchterm = Filter.SearchTerm
            });
        }
    }
}