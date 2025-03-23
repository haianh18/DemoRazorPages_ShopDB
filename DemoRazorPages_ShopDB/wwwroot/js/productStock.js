// Product Stock SignalR Client

// Create connection
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/productStockHub")
    .withAutomaticReconnect()
    .build();

// Handler for product out of stock notification
connection.on("ProductOutOfStock", (productId, productName) => {
    console.log(`Product out of stock: ${productName} (ID: ${productId})`);

    // Display notification toast
    const toast = createToast(`Sản phẩm "${productName}" đã hết hàng!`, "warning");
    showToast(toast);

    // Update UI on product list page
    updateProductListUI(productId);
});

// Handler for product quantity changed notification
connection.on("ProductQuantityChanged", (productId, newQuantity) => {
    console.log(`Product quantity changed: ID ${productId}, new quantity: ${newQuantity}`);

    // Update quantity display on product list page
    updateProductQuantityUI(productId, newQuantity);
});

// Handler for removing product from cart notification
connection.on("RemoveProductFromCart", (productId, productName) => {
    console.log(`Remove product from cart: ${productName} (ID: ${productId})`);

    // Check if we're on the cart page
    const isCartPage = window.location.pathname.includes("/Carts/");

    if (isCartPage) {
        // Find any cart item with this product ID
        const cartItems = document.querySelectorAll(`[data-product-id="${productId}"]`);

        if (cartItems.length > 0) {
            // Display notification toast
            const toast = createToast(`Sản phẩm "${productName}" đã hết hàng và đã được xóa khỏi giỏ hàng!`, "danger");
            showToast(toast);

            // Remove item from cart UI
            cartItems.forEach(item => {
                const row = item.closest('tr');
                if (row) {
                    row.classList.add('bg-danger', 'text-white', 'fade-out');
                    setTimeout(() => {
                        row.remove();
                        updateCartTotals();
                    }, 1000);
                }
            });
        }
    }
});

// Helper function to update product list UI
function updateProductListUI(productId) {
    // Check if we're on the product list page
    const isProductPage = window.location.pathname.includes("/Products/");

    if (isProductPage) {
        // Find the product row
        const productRows = document.querySelectorAll(`tr[data-product-id="${productId}"]`);

        productRows.forEach(row => {
            // Mark as out of stock
            row.classList.add('text-muted', 'bg-light');

            // Add out of stock badge if not already present
            const nameCell = row.querySelector('td:nth-child(2)');
            if (nameCell && !nameCell.querySelector('.badge.bg-danger')) {
                const badge = document.createElement('span');
                badge.className = 'badge bg-danger ms-2';
                badge.textContent = 'Hết hàng';
                nameCell.appendChild(badge);
            }

            // Disable add to cart button
            const addToCartBtn = row.querySelector('button[type="submit"]');
            if (addToCartBtn) {
                addToCartBtn.disabled = true;
                addToCartBtn.classList.remove('btn-success');
                addToCartBtn.classList.add('btn-secondary');
            }

            // Update quantity cell
            const quantityCell = row.querySelector('td:nth-child(5)');
            if (quantityCell) {
                quantityCell.textContent = '0';
            }
        });
    }
}

// Helper function to update product quantity UI
function updateProductQuantityUI(productId, newQuantity) {
    // Check if we're on the product list page
    const isProductPage = window.location.pathname.includes("/Products/");

    if (isProductPage) {
        // Find the product row
        const productRows = document.querySelectorAll(`tr[data-product-id="${productId}"]`);

        productRows.forEach(row => {
            // Update quantity cell
            const quantityCell = row.querySelector('td:nth-child(5)');
            if (quantityCell) {
                quantityCell.textContent = newQuantity;
            }

            // Update UI based on new quantity
            if (newQuantity <= 0) {
                // Already handled by out of stock event
            } else if (newQuantity < 5) {
                // Show low stock badge
                const nameCell = row.querySelector('td:nth-child(2)');
                if (nameCell) {
                    const existingBadge = nameCell.querySelector('.badge');
                    if (existingBadge) {
                        existingBadge.className = 'badge bg-warning ms-2';
                        existingBadge.textContent = 'Sắp hết';
                    } else {
                        const badge = document.createElement('span');
                        badge.className = 'badge bg-warning ms-2';
                        badge.textContent = 'Sắp hết';
                        nameCell.appendChild(badge);
                    }
                }
            }

            // Update max value for quantity input
            const quantityInput = row.querySelector('input[type="number"]');
            if (quantityInput) {
                quantityInput.max = newQuantity;
                if (parseInt(quantityInput.value) > newQuantity) {
                    quantityInput.value = newQuantity;
                }
            }
        });
    }
}

// Helper function to update cart totals
function updateCartTotals() {
    // Calculate new totals
    let totalItems = 0;
    let totalAmount = 0;

    const cartItems = document.querySelectorAll('table tbody tr:not(.fade-out)');

    cartItems.forEach(item => {
        const quantity = parseInt(item.querySelector('input[name="quantity"]')?.value || '0');
        const price = parseFloat(item.querySelector('td:nth-child(4)').textContent.replace(/[^\d.]/g, ''));

        totalItems += quantity;
        totalAmount += quantity * price;
    });

    // Update total displays
    const totalItemsElements = document.querySelectorAll('.total-items');
    const totalAmountElements = document.querySelectorAll('.total-amount');

    totalItemsElements.forEach(el => {
        el.textContent = totalItems;
    });

    totalAmountElements.forEach(el => {
        el.textContent = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(totalAmount);
    });

    // If cart is empty, reload to show empty cart UI
    if (cartItems.length === 0) {
        setTimeout(() => {
            window.location.reload();
        }, 1500);
    }
}

// Toast notification helpers
function createToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');

    toast.innerHTML = `
    <div class="d-flex">
        <div class="toast-body">
            ${message}
        </div>
        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    `;

    return toast;
}

function showToast(toastElement) {
    // Get or create toast container
    let toastContainer = document.getElementById('toast-container');

    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        document.body.appendChild(toastContainer);
    }

    // Add toast to container
    toastContainer.appendChild(toastElement);

    // Initialize and show the toast
    const bsToast = new bootstrap.Toast(toastElement, {
        animation: true,
        autohide: true,
        delay: 5000
    });

    bsToast.show();

    // Remove toast when hidden
    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    });
}

// Update connection status in UI
function updateConnectionStatus(status, message) {
    const indicator = document.getElementById('connection-indicator');
    const text = document.getElementById('connection-text');

    if (!indicator || !text) return;

    // Remove all bg-* classes
    indicator.classList.remove('bg-success', 'bg-danger', 'bg-warning', 'bg-secondary');

    switch (status) {
        case 'connected':
            indicator.classList.add('bg-success');
            indicator.title = 'Đã kết nối';
            text.textContent = 'Đã kết nối';
            break;
        case 'disconnected':
            indicator.classList.add('bg-danger');
            indicator.title = 'Mất kết nối';
            text.textContent = 'Mất kết nối';
            break;
        case 'connecting':
            indicator.classList.add('bg-warning');
            indicator.title = 'Đang kết nối';
            text.textContent = 'Đang kết nối...';
            break;
        default:
            indicator.classList.add('bg-secondary');
            indicator.title = message || 'Không xác định';
            text.textContent = message || 'Không xác định';
    }
}

// Start the connection
function startConnection() {
    updateConnectionStatus('connecting');

    connection.start()
        .then(() => {
            console.log("SignalR Connected");
            updateConnectionStatus('connected');
        })
        .catch(err => {
            console.error("SignalR Connection Error: ", err);
            updateConnectionStatus('disconnected', 'Lỗi kết nối');
            setTimeout(startConnection, 5000);
        });
}

// Reconnect if connection is lost
connection.onclose(() => {
    updateConnectionStatus('disconnected');
    setTimeout(startConnection, 2000);
});

// Start the connection when the document is ready
document.addEventListener('DOMContentLoaded', () => {
    startConnection();

    // Add data-product-id attributes to product rows
    const productRows = document.querySelectorAll('tr');
    productRows.forEach(row => {
        const productIdCell = row.querySelector('td:first-child');
        if (productIdCell && !isNaN(parseInt(productIdCell.textContent))) {
            row.setAttribute('data-product-id', productIdCell.textContent.trim());
        }
    });

    // Add data-product-id attributes to cart items
    const cartItems = document.querySelectorAll('.cart-item');
    cartItems.forEach(item => {
        const productIdInput = item.querySelector('input[name="productId"]');
        if (productIdInput) {
            item.setAttribute('data-product-id', productIdInput.value);
        }
    });
});

// Add required CSS
const style = document.createElement('style');
style.textContent = `
    .fade-out {
        opacity: 0;
        transition: opacity 1s ease-out;
    }
    
    #toast-container {
        z-index: 9999;
    }
`;
document.head.appendChild(style);

