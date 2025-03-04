using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace DemoRazorPages_ShopDB.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly OrderServices _orderServices;

        public DetailsModel(OrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var order = await _orderServices.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            Order = order;
            return Page();
        }
    }
}
