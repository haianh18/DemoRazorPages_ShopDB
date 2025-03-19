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

        public async Task OnGetAsync()
        {
            await LoadCartsAsync();
        }

        public async Task<IActionResult> OnPostCreateCartAsync()
        {
            await _cartServices.CreateCartAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteCartAsync(int cartId)
        {
            await _cartServices.DeleteCartAsync(cartId);
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