namespace DemoRazorPages_ShopDB.Models
{
    public class ProductFilterModel
    {
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchTerm { get; set; }
    }
}