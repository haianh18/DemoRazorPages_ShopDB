using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Carts
{
    public class AddToCartModel : PageModel
    {
        private readonly CartServices _cartServices;
        private readonly ProductServices _productServices;

        public AddToCartModel(
            CartServices cartServices,
            ProductServices productServices)
        {
            _cartServices = cartServices;
            _productServices = productServices;
        }

        public async Task<IActionResult> OnGetAsync(int cartId, int productId, int quantity = 1, string returnUrl = null)
        {
            try
            {
                if (quantity < 1)
                {
                    quantity = 1;
                }

                // Verify product exists and has enough stock
                var product = _productServices.GetProduct(productId);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                    return RedirectToReturnUrl(returnUrl, cartId);
                }

                if (product.Quantity < quantity)
                {
                    TempData["ErrorMessage"] = $"Số lượng sản phẩm không đủ. Chỉ còn {product.Quantity} sản phẩm.";
                    return RedirectToReturnUrl(returnUrl, cartId);
                }

                // Add to cart
                await _cartServices.AddItemToCartAsync(cartId, productId, quantity);
                TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";

                // Redirect based on returnUrl or to cart details
                return RedirectToReturnUrl(returnUrl, cartId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToReturnUrl(returnUrl, cartId);
            }
        }

        private IActionResult RedirectToReturnUrl(string returnUrl, int cartId)
        {
            // If returnUrl is provided, redirect to it
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Otherwise redirect to cart details
            return RedirectToPage("/Carts/Details", new { id = cartId });
        }
    }
}