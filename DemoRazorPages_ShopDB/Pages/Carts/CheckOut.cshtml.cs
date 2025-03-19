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

            // Lấy danh sách nhân viên
            Employees = await _employeeServices.GetEmployeesAsync();

            // Lấy danh sách khách hàng
            Customers = await _customerServices.GetCustomersAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int cartId, int customerId, int employeeId, string notes)
        {
            if (!ModelState.IsValid)
            {
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
                // Tạo đơn hàng từ giỏ hàng với khách hàng đã chọn
                var order = await _cartServices.ConvertCartToOrderAsync(
                    cartId,
                    customerId,
                    employeeId,
                    notes);

                TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công!";
                return RedirectToPage("/Orders/Details", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo đơn hàng: {ex.Message}");
                await LoadDataAsync(cartId);
                return Page();
            }
        }

        private async Task LoadDataAsync(int cartId)
        {
            Cart = await _cartServices.GetCartByIdAsync(cartId);
            CartTotal = await _cartServices.GetCartTotalAsync(cartId);
            Employees = await _employeeServices.GetEmployeesAsync();
            Customers = await _customerServices.GetCustomersAsync();
        }
    }
}