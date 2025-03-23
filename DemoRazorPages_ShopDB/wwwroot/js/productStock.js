// SignalR Debugging Script

// Cấu hình Toastr
toastr.options = {
    "closeButton": true,
    "debug": true,  // Bật chế độ debug
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// Hàm hiển thị thông báo nâng cao
function advancedNotification(title, message, type = 'info') {
    console.log(`[${type.toUpperCase()}] ${title}: ${message}`);

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

// Tạo kết nối SignalR với nhiều log chi tiết
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/productStockHub")
    .configureLogging(signalR.LogLevel.Debug)  // Bật log chi tiết
    .withAutomaticReconnect([0, 1000, 5000, null])
    .build();

// Quản lý trạng thái kết nối
function updateConnectionStatus(status, message) {
    console.log(`Connection Status: ${status}, Message: ${message}`);

    const indicator = document.getElementById('connection-indicator');
    const text = document.getElementById('connection-text');

    if (!indicator || !text) {
        console.warn('Connection status elements not found');
        return;
    }

    // Xóa các lớp trạng thái cũ
    indicator.classList.remove('bg-success', 'bg-danger', 'bg-warning', 'bg-secondary');

    switch (status) {
        case 'connected':
            indicator.classList.add('bg-success');
            indicator.title = 'Đã kết nối';
            text.textContent = 'Đã kết nối';
            advancedNotification('Kết Nối', 'Đã thiết lập kết nối với máy chủ', 'success');
            break;
        case 'disconnected':
            indicator.classList.add('bg-danger');
            indicator.title = 'Mất kết nối';
            text.textContent = 'Mất kết nối';
            advancedNotification('Ngắt Kết Nối', 'Mất kết nối với máy chủ', 'error');
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

// Bắt đầu kết nối SignalR
async function startConnection() {
    try {
        console.log('Attempting to start SignalR connection...');
        updateConnectionStatus('connecting');

        await connection.start();
        console.log('SignalR Connected successfully');
        updateConnectionStatus('connected');
    } catch (err) {
        console.error('SignalR Connection Error:', err);
        updateConnectionStatus('disconnected', err.message);

        // Thử kết nối lại sau 5 giây
        setTimeout(startConnection, 5000);
    }
}

// Đăng ký các sự kiện
connection.onclose(async (error) => {
    console.log('Connection closed:', error);
    updateConnectionStatus('disconnected', error ? error.message : 'Kết nối bị đóng');
    await startConnection();
});

connection.onreconnecting(error => {
    console.log('Reconnecting:', error);
    updateConnectionStatus('connecting', 'Đang kết nối lại...');
});

connection.onreconnected(connectionId => {
    console.log('Reconnected. Connected with id:', connectionId);
    updateConnectionStatus('connected');
});

// Đăng ký sự kiện từ server
connection.on("TestConnectionMessage", (message) => {
    console.log('Test Connection Message Received:', message);
    advancedNotification('Thông Báo Từ Server', message, 'info');
});

// Xử lý nút kiểm tra
document.addEventListener('DOMContentLoaded', () => {
    const testSignalRButton = document.getElementById('testSignalR');

    if (testSignalRButton) {
        testSignalRButton.addEventListener('click', async function () {
            console.log('Test SignalR Button Clicked');

            try {
                // Kiểm tra trạng thái kết nối
                if (connection.state !== signalR.HubConnectionState.Connected) {
                    console.warn('Connection not in Connected state. Current state:', connection.state);
                    advancedNotification('Lỗi Kết Nối', 'Chưa thiết lập kết nối SignalR', 'error');
                    await startConnection();
                    return;
                }

                // Gửi tin nhắn kiểm tra
                await connection.invoke('TestSignalRConnection', 'Kiểm tra kết nối từ client');
                console.log('Test message sent successfully');
                advancedNotification('Kiểm Tra SignalR', 'Kết nối hoạt động bình thường', 'success');
            } catch (err) {
                console.error('Error testing SignalR connection:', err);
                advancedNotification('Lỗi SignalR', `Không thể gửi tin nhắn: ${err.message}`, 'error');
            }
        });
    } else {
        console.warn('Test SignalR button not found');
    }

    // Bắt đầu kết nối khi trang tải
    startConnection();
});