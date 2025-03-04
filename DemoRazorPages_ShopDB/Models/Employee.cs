using System;
using System.Collections.Generic;

namespace DemoRazorPages_ShopDB.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
