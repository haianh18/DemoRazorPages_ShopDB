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
                return NotFound();
            }

            Cart = cart;
            Products = await _productServices.GetAllProductsAsync();
            CartTotal = await _cartServices.GetCartTotalAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAddItemAsync(int cartId, int productId, int quantity)
        {
            await _cartServices.AddItemToCartAsync(cartId, productId, quantity);
            return RedirectToPage(new { id = cartId });
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartId, int cartItemId, int quantity)
        {
            await _cartServices.UpdateCartItemQuantityAsync(cartItemId, quantity);
            return RedirectToPage(new { id = cartId });
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int cartId, int cartItemId)
        {
            await _cartServices.RemoveItemFromCartAsync(cartItemId);
            return RedirectToPage(new { id = cartId });
        }

        public async Task<IActionResult> OnPostClearCartAsync(int cartId)
        {
            await _cartServices.ClearCartAsync(cartId);
            return RedirectToPage(new { id = cartId });
        }
    }
}