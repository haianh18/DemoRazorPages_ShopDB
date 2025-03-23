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

        // Ghi log khi có kết nối mới
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"SignalR Connection: {Context.ConnectionId} connected.");
            await base.OnConnectedAsync();
        }

        // Ghi log khi ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"SignalR Connection: {Context.ConnectionId} disconnected.");
            if (exception != null)
            {
                Console.WriteLine($"Disconnection error: {exception.Message}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Phương thức kiểm tra kết nối
        public async Task TestSignalRConnection(string message)
        {
            Console.WriteLine($"Test message received: {message}");

            // Gửi phản hồi tới tất cả client
            await Clients.All.SendAsync("TestConnectionMessage",
                $"Server received: {message} at {DateTime.Now}");
        }
    }
}