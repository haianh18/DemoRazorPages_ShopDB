using DemoRazorPages_ShopDB.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoRazorPages_ShopDB.Services
{
    public class ProductServices
    {
        ShopDbrazorPagesContext _context;

        public ProductServices(ShopDbrazorPagesContext context)
        {
            _context = context;
        }

        public List<Product> GetProducts(int? CategoryId)
        {
            if (CategoryId is null)
                return _context.Products
                    .Include(p => p.Category)
                    .ToList();
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == CategoryId)
                .ToList();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByOrderId(int OrderId)
        {
            return await _context.OrderDetails
                .Where(d => d.OrderId == OrderId)
                .Select(d => d.Product!)
                .ToListAsync();
        }

        public Product? GetProduct(int ProductId)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == ProductId);
        }

        public bool DeleteProduct(int ProductId)
        {
            Product? product = GetProduct(ProductId);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddProduct(Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdateProduct(Product product)
        {
            try
            {
                Product? newProduct = GetProduct(product.ProductId);
                if (newProduct != null)
                {
                    newProduct.ProductName = product.ProductName;
                    newProduct.CategoryId = product.CategoryId;
                    newProduct.Price = product.Price;
                    newProduct.Quantity = product.Quantity;
                    _context.Products.Update(newProduct);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }


        }

    }
}
