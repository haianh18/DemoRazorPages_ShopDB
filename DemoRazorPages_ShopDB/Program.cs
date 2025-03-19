using DemoRazorPages_ShopDB.Models;

namespace DemoRazorPages_ShopDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddScoped<ShopDbrazorPagesContext>();
            builder.Services.AddScoped<Services.EmployeeServices>();
            builder.Services.AddScoped<Services.CustomerServices>();
            builder.Services.AddScoped<Services.OrderServices>();
            builder.Services.AddScoped<Services.CategoryServices>();
            builder.Services.AddScoped<Services.ProductServices>();

            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.MapRazorPages();
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Orders/List");
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}
