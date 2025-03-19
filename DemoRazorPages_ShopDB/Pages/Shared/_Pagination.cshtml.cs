using DemoRazorPages_ShopDB.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazorPages_ShopDB.Pages.Shared
{
    public class PaginationModel<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; } = new List<T>();

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public object Filter { get; set; }

    }
}
