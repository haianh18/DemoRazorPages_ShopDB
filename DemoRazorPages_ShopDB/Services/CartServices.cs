using DemoRazorPages_ShopDB.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoRazorPages_ShopDB.Services
{
    public class CartServices
    {
        private readonly ShopDbrazorPagesContext _context;
        private readonly ProductServices _productServices;

        public CartServices(ShopDbrazorPagesContext context, ProductServices productServices)
        {
            _context = context;
            _productServices = productServices;
        }

        // Get all carts with their items and related products
        public async Task<List<Cart>> GetAllCartsAsync()
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        // Get cart by ID with related items and products
        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        // Create a new cart
        public async Task<Cart> CreateCartAsync()
        {
            var cart = new Cart
            {
                CreatedDate = DateTime.Now
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        // Add or update an item in the cart
        public async Task<CartItem> AddItemToCartAsync(int cartId, int productId, int quantity = 1)
        {
            // Validation
            if (quantity <= 0)
            {
                throw new ArgumentException("Số lượng phải lớn hơn 0");
            }

            var cart = await GetCartByIdAsync(cartId);
            if (cart == null)
            {
                throw new ArgumentException("Không tìm thấy giỏ hàng");
            }

            var product = _productServices.GetProduct(productId);
            if (product == null)
            {
                throw new ArgumentException("Không tìm thấy sản phẩm");
            }

            if (product.Quantity < quantity)
            {
                throw new ArgumentException($"Số lượng sản phẩm trong kho không đủ (Có: {product.Quantity})");
            }

            // Check if product already exists in cart
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                // Update quantity if product exists
                existingItem.Quantity += quantity;

                // Check if updated quantity exceeds available stock
                if (existingItem.Quantity > product.Quantity)
                {
                    throw new ArgumentException($"Tổng số lượng trong giỏ vượt quá số lượng có sẵn (Có: {product.Quantity})");
                }

                _context.CartItems.Update(existingItem);
                await _context.SaveChangesAsync();
                return existingItem;
            }
            else
            {
                // Add new cart item with current product price
                var newItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price // Store current price when adding to cart
                };

                _context.CartItems.Add(newItem);
                await _context.SaveChangesAsync();
                return newItem;
            }
        }

        // Update cart item quantity
        public async Task UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
            {
                throw new ArgumentException("Không tìm thấy sản phẩm trong giỏ hàng");
            }

            if (quantity <= 0)
            {
                // Remove item if quantity is zero or negative
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                // Validate quantity against available stock
                if (quantity > cartItem.Product.Quantity)
                {
                    throw new ArgumentException($"Số lượng yêu cầu vượt quá số lượng có sẵn (Có: {cartItem.Product.Quantity})");
                }

                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        // Remove an item from cart
        public async Task RemoveItemFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        // Clear all items from a cart
        public async Task ClearCartAsync(int cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        // Delete a cart and all its items
        public async Task DeleteCartAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        // Calculate cart total value
        public async Task<decimal> GetCartTotalAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .SumAsync(ci => ci.Price * ci.Quantity);
        }

        // Convert cart to order
        public async Task<Order> ConvertCartToOrderAsync(int cartId, int customerId, int employeeId, string? orderNote = null)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new ArgumentException("Giỏ hàng trống hoặc không tồn tại");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create new order with the note
                var order = new Order
                {
                    CustomerId = customerId,
                    EmployeeId = employeeId,
                    OrderDate = DateTime.Now,
                    OrderNote = orderNote,
                    OrderDetails = new List<OrderDetail>()
                };

                // Add order details from cart items
                foreach (var item in cart.CartItems)
                {
                    // Get current product to check stock
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product == null)
                    {
                        throw new Exception($"Sản phẩm với ID {item.ProductId} không tồn tại");
                    }

                    if (product.Quantity < item.Quantity)
                    {
                        throw new Exception($"Sản phẩm '{product.ProductName}' không đủ số lượng trong kho (Yêu cầu: {item.Quantity}, Có sẵn: {product.Quantity})");
                    }

                    // Add to order details
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });

                    // Update product quantity
                    product.Quantity -= item.Quantity;
                    _context.Products.Update(product);
                }

                // Save order
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Clear the cart after successful order creation
                await ClearCartAsync(cartId);

                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}