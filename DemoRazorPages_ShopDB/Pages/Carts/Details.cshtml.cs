using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Carts
{
    public class DetailsModel : PageModel
    {
        private readonly CartServices _cartServices;
        private readonly ProductServices _productServices;

        public DetailsModel(
            CartServices cartServices,
            ProductServices productServices)
        {
            _cartServices = cartServices;
            _productServices = productServices;
        }

        public Cart Cart { get; set; } = new Cart();
        public List<Product> Products { get; set; } = new List<Product>();
        public decimal CartTotal { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cart = await _cartServices.GetCartByIdAsync(id);
            if (cart == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy giỏ hàng.";
                return RedirectToPage("./Index");
            }

            Cart = cart;
            await LoadProductsAsync();
            CartTotal = await _cartServices.GetCartTotalAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAddItemAsync(int cartId, int productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    throw new ArgumentException("Số lượng phải lớn hơn 0");
                }

                await _cartServices.AddItemToCartAsync(cartId, productId, quantity);
                TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage(new { id = cartId });
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartId, int cartItemId, int quantity)
        {
            try
            {
                await _cartServices.UpdateCartItemQuantityAsync(cartItemId, quantity);
                TempData["SuccessMessage"] = "Số lượng đã được cập nhật!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage(new { id = cartId });
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int cartId, int cartItemId)
        {
            try
            {
                await _cartServices.RemoveItemFromCartAsync(cartItemId);
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage(new { id = cartId });
        }

        public async Task<IActionResult> OnPostClearCartAsync(int cartId)
        {
            try
            {
                await _cartServices.ClearCartAsync(cartId);
                TempData["SuccessMessage"] = "Đã xóa tất cả sản phẩm trong giỏ hàng!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage(new { id = cartId });
        }

        private async Task LoadProductsAsync()
        {
            // Load only products that have quantity > 0
            var allProducts = await _productServices.GetAllProductsAsync();
            Products = allProducts.Where(p => p.Quantity > 0).ToList();
        }
    }
}