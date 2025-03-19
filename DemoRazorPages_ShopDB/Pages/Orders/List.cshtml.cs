using DemoRazorPages_ShopDB.Models;
using DemoRazorPages_ShopDB.Pages.Shared;
using DemoRazorPages_ShopDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Orders
{
    public class ListModel : PageModel
    {
        private readonly OrderServices _orderServices;
        private readonly CustomerServices _customerServices;
        private readonly EmployeeServices _employeeServices;
        private readonly ProductServices _productServices;

        public ListModel(OrderServices orderServices, CustomerServices customerServices, EmployeeServices employeeServices, ProductServices productServices)
        {
            _orderServices = orderServices;
            _customerServices = customerServices;
            _employeeServices = employeeServices;
            _productServices = productServices;
        }

        [BindProperty(SupportsGet = true)]
        public FilterModel Filter { get; set; } = new FilterModel();

        public List<Order>? Orders { get; set; }
        public List<Employee>? Employees { get; set; }
        public List<Customer>? Customers { get; set; }
        public List<Product>? Products { get; set; }

        public PaginationModel<Order> Pagination { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            Employees = await _employeeServices.GetEmployeesAsync();
            Customers = await _customerServices.GetCustomersAsync();
            Products = await _productServices.GetAllProductsAsync();
            Orders = await _orderServices.GetAllOrderAsync(Filter);

            if (Orders != null)
            {
                int pageSize = 10;
                int currentPageIndex = pageIndex ?? 1;
                int totalOrders = Orders.Count;

                var orders = Orders.Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList();

                Pagination = new PaginationModel<Order>
                {
                    PageIndex = currentPageIndex,
                    TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize),
                    Items = orders,
                    Filter = Filter
                };
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int orderId, int? employeeid, int? customerid, int? productid, DateTime? fromdate, DateTime? todate)
        {
            await _orderServices.Delete(orderId);

            //// Preserve filter values
            Filter.EmployeeId = employeeid;
            Filter.CustomerId = customerid;
            Filter.ProductId = productid;
            Filter.FromDate = fromdate;
            Filter.ToDate = todate;

            return RedirectToPage(new
            {
                employeeid = Filter.EmployeeId,
                customerid = Filter.CustomerId,
                productid = Filter.ProductId,
                fromdate = Filter.FromDate?.ToString("yyyy-MM-dd"),
                todate = Filter.ToDate?.ToString("yyyy-MM-dd")
            });
            //return RedirectToPage("List");
        }
    }

}

