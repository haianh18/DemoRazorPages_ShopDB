using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Carts
{
    public class IndexModel : PageModel
    {
        private readonly CartServices _cartServices;

        public IndexModel(CartServices cartServices)
        {
            _cartServices = cartServices;
        }

        public List<Cart> Carts { get; set; } = new List<Cart>();
        public Dictionary<int, decimal> CartTotals { get; set; } = new Dictionary<int, decimal>();

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadCartsAsync();
        }

        public async Task<IActionResult> OnPostCreateCartAsync()
        {
            try
            {
                var cart = await _cartServices.CreateCartAsync();
                TempData["SuccessMessage"] = "Giỏ hàng mới đã được tạo thành công!";

                // Redirect to the details page for the new cart
                return RedirectToPage("./Details", new { id = cart.CartId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostDeleteCartAsync(int cartId)
        {
            try
            {
                await _cartServices.DeleteCartAsync(cartId);
                TempData["SuccessMessage"] = "Giỏ hàng đã được xóa thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task LoadCartsAsync()
        {
            Carts = await _cartServices.GetAllCartsAsync();

            foreach (var cart in Carts)
            {
                decimal total = await _cartServices.GetCartTotalAsync(cart.CartId);
                CartTotals[cart.CartId] = total;
            }
        }
    }
}