using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Carts
{
    public class CheckoutModel : PageModel
    {
        private readonly CartServices _cartServices;
        private readonly EmployeeServices _employeeServices;
        private readonly CustomerServices _customerServices;

        public CheckoutModel(
            CartServices cartServices,
            EmployeeServices employeeServices,
            CustomerServices customerServices)
        {
            _cartServices = cartServices;
            _employeeServices = employeeServices;
            _customerServices = customerServices;
        }

        public Cart Cart { get; set; } = new Cart();
        public decimal CartTotal { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Customer> Customers { get; set; } = new List<Customer>();

        [BindProperty]
        public int SelectedCustomerId { get; set; }

        [BindProperty]
        public int SelectedEmployeeId { get; set; }

        [BindProperty]
        public string? OrderNote { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cart = await _cartServices.GetCartByIdAsync(id);
            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống hoặc không tồn tại.";
                return RedirectToPage("/Carts/Index");
            }

            Cart = cart;
            CartTotal = await _cartServices.GetCartTotalAsync(id);

            // Load employees and customers
            await LoadDataAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int cartId, int customerId, int employeeId, string? orderNote)
        {
            // Validation
            if (customerId == 0)
            {
                ModelState.AddModelError("customerId", "Vui lòng chọn khách hàng.");
            }

            if (employeeId == 0)
            {
                ModelState.AddModelError("employeeId", "Vui lòng chọn nhân viên phụ trách.");
            }

            if (!ModelState.IsValid)
            {
                SelectedCustomerId = customerId;
                SelectedEmployeeId = employeeId;
                OrderNote = orderNote;
                await LoadDataAsync(cartId);
                return Page();
            }

            var cart = await _cartServices.GetCartByIdAsync(cartId);
            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống hoặc không tồn tại.";
                return RedirectToPage("/Carts/Index");
            }

            try
            {
                // Create order from cart with the order note
                var order = await _cartServices.ConvertCartToOrderAsync(
                    cartId,
                    customerId,
                    employeeId,
                    orderNote);

                TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công!";
                return RedirectToPage("/Orders/Details", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tạo đơn hàng: {ex.Message}";

                // Preserve user selections
                SelectedCustomerId = customerId;
                SelectedEmployeeId = employeeId;
                OrderNote = orderNote;

                await LoadDataAsync(cartId);
                return Page();
            }
        }

        private async Task LoadDataAsync(int? cartId = null)
        {
            // If cart ID is provided, load the cart
            if (cartId.HasValue)
            {
                Cart = await _cartServices.GetCartByIdAsync(cartId.Value);
                CartTotal = await _cartServices.GetCartTotalAsync(cartId.Value);
            }

            // Load employees and customers
            Employees = await _employeeServices.GetEmployeesAsync();
            Customers = await _customerServices.GetCustomersAsync();
        }
    }
}