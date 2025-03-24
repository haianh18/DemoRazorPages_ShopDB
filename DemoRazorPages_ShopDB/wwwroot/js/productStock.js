// SignalR Connection Manager for Product Stock

// Create SignalR connection with minimal configuration
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/productStockHub")
    .configureLogging(signalR.LogLevel.Error) // Only log errors
    .withAutomaticReconnect([0, 2000, 10000, 30000, null])
    .build();

// Start SignalR connection silently
async function startConnection() {
    try {
        await connection.start();
        console.log('SignalR connected');
    } catch (err) {
        console.error('SignalR connection error:', err);
        // Try reconnecting after 5 seconds
        setTimeout(startConnection, 5000);
    }
}


// Start connection when page loads
document.addEventListener('DOMContentLoaded', () => {
    startConnection();
});

// Notification Center with Toastr Integration
document.addEventListener('DOMContentLoaded', () => {
    // Configure Toastr settings
    toastr.options = {
        "closeButton": true,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "2000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    // Listen for SignalR events to show notifications if SignalR connection is available
    if (typeof connection !== 'undefined') {
        // Product out of stock notification
        connection.on("ProductOutOfStock", (productId, productName) => {
            toastr.warning(
                `Sản phẩm "${productName}" đã hết hàng. Trang sẽ được làm mới để cập nhật thông tin.`,
                "Sản phẩm hết hàng",
                {
                    timeOut: 5000,
                    onHidden: function () {
                        // Reload the page to update product status
                        location.reload();
                    }
                }
            );
        });

        // Product removed from cart notification
        connection.on("RemoveProductFromCart", (productId, productName) => {
            // Only show this notification on cart pages
            if (window.location.pathname.includes("/Carts/")) {
                toastr.error(
                    `Sản phẩm "${productName}" đã hết hàng và đã bị xóa khỏi giỏ hàng. Trang sẽ được làm mới.`,
                    "Sản phẩm đã bị xóa",
                    {
                        timeOut: 3000,
                        onHidden: function () {
                            // Reload the page to update cart contents
                            location.reload();
                        }
                    }
                );
            }
        });

        // Product quantity changed notification
        connection.on("ProductQuantityChanged", (productId, newQuantity) => {
            if (newQuantity <= 5 && newQuantity > 0) {
                toastr.info(
                    `Sản phẩm có ID: ${productId} chỉ còn ${newQuantity} trong kho.`,
                    "Sản phẩm sắp hết hàng",
                    {
                        timeOut: 3000,
                        onHidden: function () {
                            location.reload();
                        }
                    }
                );
            } else {
                toastr.info(
                    "Số lượng sản phẩm đã thay đổi. Trang sẽ được làm mới.", "Số lượng sản phẩm đã thay đổi",
                    {
                        timeOut: 3000,
                        onHidden: function () {
                            location.reload();
                        }
                    }
                );
            }
        });

        // Cart deleted notification
        connection.on("CartDeleted", (cartId) => {
            toastr.error(
                `Giỏ hàng #${cartId} đã bị xóa. Trang sẽ được làm mới.`,
                "Giỏ hàng đã bị xóa",
                {
                    timeOut: 3000,
                    onHidden: function () {
                        // If we're on a carts page, reload it
                        location.reload();
                    }
                }
            );
        });

        // Order deleted notification
        connection.on("OrderDeleted", (orderId) => {
            toastr.error(
                `Đơn hàng #${orderId} đã bị xóa. Trang sẽ được làm mới.`,
                "Đơn hàng đã bị xóa",
                {
                    timeOut: 3000,
                    onHidden: function () {
                        location.reload();
                    }
                }
            );
        });

        // Product Price changed notification
        connection.on("ProductPriceChanged", (productId, newPrice, oldPrice) => {
            console.log("ProductPriceChanged");
            const formattedNewPrice = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(newPrice);
            const formattedOldPrice = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(oldPrice);

            toastr.info(
                `Giá của sản phẩm có ID: ${productId} đã thay đổi từ ${formattedOldPrice} thành ${formattedNewPrice}. Trang sẽ được làm mới.`,
                "Giá sản phẩm đã thay đổi",
                {
                    timeOut: 3000,
                    onHidden: function () {
                        location.reload();
                    }
                }
            );
        });

        // Product Name changed notification
        connection.on("ProductNameChanged", (productId, newName, oldName) => {
            toastr.info(
                `Tên sản phẩm có ID: ${productId} đã thay đổi từ "${oldName}" thành "${newName}". Trang sẽ được làm mới.`,
                "Tên sản phẩm đã thay đổi",
                {
                    timeOut: 3000,
                    onHidden: function () {
                        location.reload();
                    }
                }
            );
        });
    }
});

// Function to show a notification - can be called from other parts of the code
function showNotification(title, message, type = 'info') {
    switch (type) {
        case 'success':
            toastr.success(message, title);
            break;
        case 'warning':
            toastr.warning(message, title);
            break;
        case 'error':
        case 'danger':
            toastr.error(message, title);
            break;
        case 'info':
        default:
            toastr.info(message, title);
    }
}