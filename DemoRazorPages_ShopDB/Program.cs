using DemoRazorPages_ShopDB.Hubs;
using DemoRazorPages_ShopDB.Models;

namespace DemoRazorPages_ShopDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddRazorPages();

            // Add SignalR
            builder.Services.AddSignalR();

            // Add application services
            builder.Services.AddScoped<ShopDbrazorPagesContext>();
            builder.Services.AddScoped<Services.EmployeeServices>();
            builder.Services.AddScoped<Services.CustomerServices>();
            builder.Services.AddScoped<Services.OrderServices>();
            builder.Services.AddScoped<Services.CategoryServices>();
            builder.Services.AddScoped<Services.ProductServices>();
            builder.Services.AddScoped<Services.CartServices>();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Map SignalR hub
            app.MapHub<ProductStockHub>("/productStockHub");

            app.MapRazorPages();

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Products/List");
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}