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
    }
}