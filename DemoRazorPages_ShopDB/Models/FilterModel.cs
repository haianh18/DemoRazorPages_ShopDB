namespace DemoRazorPages_ShopDB.Models
{
    public class FilterModel
    {
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
