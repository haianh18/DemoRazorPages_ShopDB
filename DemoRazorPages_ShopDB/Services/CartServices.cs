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

        // Lấy tất cả giỏ hàng
        public async Task<List<Cart>> GetAllCartsAsync()
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        // Lấy thông tin một giỏ hàng theo ID
        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        // Tạo giỏ hàng mới
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

        // Thêm sản phẩm vào giỏ hàng
        public async Task<CartItem> AddItemToCartAsync(int cartId, int productId, int quantity = 1)
        {
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

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                // Cập nhật số lượng nếu sản phẩm đã tồn tại
                existingItem.Quantity += quantity;
                _context.CartItems.Update(existingItem);
                await _context.SaveChangesAsync();
                return existingItem;
            }
            else
            {
                // Thêm mới sản phẩm vào giỏ hàng với giá hiện tại của sản phẩm
                var newItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price // Lưu giá sản phẩm tại thời điểm thêm vào giỏ
                };

                _context.CartItems.Add(newItem);
                await _context.SaveChangesAsync();
                return newItem;
            }
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public async Task UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                throw new ArgumentException("Không tìm thấy sản phẩm trong giỏ hàng");
            }

            if (quantity <= 0)
            {
                // Xóa sản phẩm nếu số lượng <= 0
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        // Xóa một sản phẩm khỏi giỏ hàng
        public async Task RemoveItemFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        // Xóa tất cả sản phẩm trong giỏ hàng
        public async Task ClearCartAsync(int cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        // Xóa một giỏ hàng
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

        // Tính tổng giá trị giỏ hàng
        public async Task<decimal> GetCartTotalAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .SumAsync(ci => ci.Price * ci.Quantity);
        }

        // Chuyển đổi giỏ hàng thành đơn hàng
        public async Task<Order> ConvertCartToOrderAsync(int cartId, int customerId, int employeeId, string notes = null)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new ArgumentException("Giỏ hàng trống hoặc không tồn tại");
            }

            // Tạo đơn hàng mới
            var order = new Order
            {
                CustomerId = customerId,
                EmployeeId = employeeId,
                OrderDate = DateTime.Now,
                OrderDetails = new List<OrderDetail>()
            };

            // Thêm chi tiết đơn hàng từ giỏ hàng
            foreach (var item in cart.CartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            // Thêm đơn hàng vào cơ sở dữ liệu
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }
    }
}