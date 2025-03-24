using DemoRazorPages_ShopDB.Hubs;
using DemoRazorPages_ShopDB.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DemoRazorPages_ShopDB.Services
{
    public class ProductServices
    {
        private readonly ShopDbrazorPagesContext _context;
        private readonly IHubContext<ProductStockHub> _hubContext;

        public ProductServices(
            ShopDbrazorPagesContext context,
            IHubContext<ProductStockHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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

        public List<Product> GetFilteredProducts(ProductFilterModel filter)
        {
            IQueryable<Product> query = _context.Products.Include(p => p.Category);

            if (filter != null)
            {
                if (filter.CategoryId > 0)
                    query = query.Where(p => p.CategoryId == filter.CategoryId);

                if (filter.MinPrice != null)
                    query = query.Where(p => p.Price >= filter.MinPrice);

                if (filter.MaxPrice != null)
                    query = query.Where(p => p.Price <= filter.MaxPrice);

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    string searchTerm = filter.SearchTerm.ToLower();
                    query = query.Where(p => p.ProductName.ToLower().Contains(searchTerm));
                }
            }

            return query.ToList();
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
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == ProductId);
        }

        public bool DeleteProduct(int ProductId)
        {
            Product? product = GetProduct(ProductId);
            if (product != null)
            {
                // Check if product is referenced in any order details
                bool isUsedInOrders = _context.OrderDetails.Any(od => od.ProductId == ProductId);

                if (isUsedInOrders)
                {
                    // Product is in use, can't delete
                    return false;
                }

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

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                Product? existingProduct = GetProduct(product.ProductId);
                if (existingProduct != null)
                {
                    // Check if the product is going out of stock
                    bool wasInStock = existingProduct.Quantity > 0;
                    bool willBeOutOfStock = product.Quantity <= 0;

                    // Store original values for comparison
                    int originalQuantity = existingProduct.Quantity;
                    decimal originalPrice = existingProduct.Price;

                    // Update the product
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.Price = product.Price;
                    existingProduct.Quantity = product.Quantity;
                    _context.Products.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    // Handle notifications if product quantity changed
                    if (originalQuantity != product.Quantity)
                    {
                        // If product went from in-stock to out-of-stock
                        if (wasInStock && willBeOutOfStock)
                        {
                            // Send out-of-stock notification
                            await _hubContext.Clients.All.SendAsync("ProductOutOfStock",
                                product.ProductId, product.ProductName);

                            // Remove this product from all carts
                            await RemoveProductFromAllCartsAsync(product.ProductId, product.ProductName);
                        }

                        // Otherwise just notify about quantity change
                        else
                        {
                            await _hubContext.Clients.All.SendAsync("ProductQuantityChanged",
                                product.ProductId, product.Quantity);
                        }
                    }

                    if (originalPrice != product.Price)
                    {
                        await _hubContext.Clients.All.SendAsync("ProductPriceChanged",
                            product.ProductId);
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Helper method to remove a product from all carts when it becomes out of stock manually
        private async Task RemoveProductFromAllCartsAsync(int productId, string productName)
        {
            // Find all cart items containing this product
            var cartItems = await _context.CartItems
                .Where(ci => ci.ProductId == productId)
                .ToListAsync();

            if (cartItems.Any())
            {
                // Remove items from carts
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                // Notify clients
                await _hubContext.Clients.All.SendAsync("RemoveProductFromCart",
                    productId, productName);
            }
        }
    }
}