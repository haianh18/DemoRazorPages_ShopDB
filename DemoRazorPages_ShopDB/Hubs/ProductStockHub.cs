using Microsoft.AspNetCore.SignalR;

namespace DemoRazorPages_ShopDB.Hubs
{
    public class ProductStockHub : Hub
    {
        public async Task NotifyProductOutOfStock(int productId, string productName)
        {
            await Clients.Others.SendAsync("ProductOutOfStock", productId, productName);
        }

        public async Task NotifyProductQuantityChanged(int productId, int newQuantity)
        {
            await Clients.Others.SendAsync("ProductQuantityChanged", productId, newQuantity);
        }

        public async Task NotifyRemoveProductFromCart(int productId, string productName)
        {
            await Clients.Others.SendAsync("RemoveProductFromCart", productId, productName);
        }

        // New notification method for cart deletion
        public async Task NotifyCartDeleted(int cartId)
        {
            await Clients.Others.SendAsync("CartDeleted", cartId);
        }

        // New notification method for order deletion
        public async Task NotifyOrderDeleted(int orderId)
        {
            await Clients.Others.SendAsync("OrderDeleted", orderId);
        }

        // Connection logging
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"SignalR Connection: {Context.ConnectionId} connected.");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"SignalR Connection: {Context.ConnectionId} disconnected.");
            if (exception != null)
            {
                Console.WriteLine($"Disconnection error: {exception.Message}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}