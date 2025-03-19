using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Carts
{
    public class AddToCartModel : PageModel
    {
        private readonly CartServices _cartServices;

        public AddToCartModel(CartServices cartServices)
        {
            _cartServices = cartServices;
        }

        public async Task<IActionResult> OnGetAsync(int cartId, int productId, int quantity = 1)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            await _cartServices.AddItemToCartAsync(cartId, productId, quantity);

            TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";

            // Redirect to cart details
            return RedirectToPage("/Carts/Details", new { id = cartId });
        }
    }
}