-- Tạo cơ sở dữ liệu
CREATE DATABASE ShopDBRazorPages;
GO

USE ShopDBRazorPages;
GO

-- Tạo bảng Category
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);
GO

-- Tạo bảng Employee
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL
);
GO

-- Tạo bảng Customer
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15) NOT NULL
);
GO

-- Tạo bảng Product
CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID),
    Price DECIMAL(18, 2) NOT NULL,
    Quantity INT NOT NULL
);
GO

-- Tạo bảng Order
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customer(CustomerID),
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    OrderDate DATETIME DEFAULT GETDATE()
);
GO

-- Tạo bảng OrderDetail (liên kết Order và Product)
CREATE TABLE OrderDetail (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Quantity INT NOT NULL
);
GO

-- Chèn dữ liệu vào bảng Category (không tuân theo thứ tự)
INSERT INTO Category (CategoryName) VALUES
('Books'),
('Electronics'),
('Clothing'),
('Home & Kitchen'),
('Toys'),
('Sports'),
('Beauty'),
('Health'),
('Automotive'),
('Grocery');
GO

-- Chèn dữ liệu vào bảng Employee (không tuân theo thứ tự)
INSERT INTO Employee (EmployeeName, Position) VALUES
('David Johnson', 'Software Engineer'),
('Emily Clark', 'Product Manager'),
('George Walker', 'Data Analyst'),
('Hannah Lewis', 'UX Designer'),
('Ian Hall', 'DevOps Engineer'),
('Julia Young', 'Content Writer'),
('Kevin King', 'System Administrator'),
('Laura Allen', 'Graphic Designer'),
('Michael Scott', 'Project Manager'),
('Nancy Green', 'Business Analyst'),
('Oliver Wright', 'Network Engineer'),
('Patricia Harris', 'Sales Manager'),
('Quincy Adams', 'Technical Support'),
('Rachel Martinez', 'HR Coordinator'),
('Samuel Nelson', 'Finance Analyst'),
('Tina Brown', 'Customer Support'),
('Uma Patel', 'Quality Assurance'),
('Victor Lee', 'Database Administrator'),
('Wendy Garcia', 'Marketing Manager'),
('Xander Lopez', 'Security Analyst');
GO

-- Chèn dữ liệu vào bảng Customer (không tuân theo thứ tự)
INSERT INTO Customer (CustomerName, Email, Phone) VALUES
('Ryan Howard', 'ryan@example.com', '123-456-7891'),
('Kelly Kapoor', 'kelly@example.com', '234-567-8902'),
('Toby Flenderson', 'toby@example.com', '345-678-9013'),
('Darryl Philbin', 'darryl@example.com', '456-789-0124'),
('Erin Hannon', 'erin@example.com', '567-890-1235'),
('Gabe Lewis', 'gabe@example.com', '678-901-2346'),
('Holly Flax', 'holly@example.com', '789-012-3457'),
('Jan Levinson', 'jan@example.com', '890-123-4568'),
('Robert California', 'robert@example.com', '901-234-5679'),
('Nellie Bertram', 'nellie@example.com', '012-345-6780'),
('Clark Green', 'clark@example.com', '123-456-7892'),
('Meredith Palmer', 'meredith@example.com', '234-567-8903'),
('Creed Bratton', 'creed@example.com', '345-678-9014'),
('Jo Bennett', 'jo@example.com', '456-789-0125'),
('Todd Packer', 'todd@example.com', '567-890-1236'),
('Karen Filippelli', 'karen@example.com', '678-901-2347'),
('Roy Anderson', 'roy@example.com', '789-012-3458'),
('Pete Miller', 'pete@example.com', '890-123-4569'),
('Charles Miner', 'charles@example.com', '901-234-5670'),
('Deangelo Vickers', 'deangelo@example.com', '012-345-6781');
GO

-- Chèn dữ liệu vào bảng Product (không tuân theo thứ tự)
INSERT INTO Product (ProductName, CategoryID, Price, Quantity) VALUES
('Tablet', 2, 300.00, 15),
('Hoodie', 3, 35.00, 40),
('Mystery Book', 1, 20.00, 25),
('Toaster', 4, 40.00, 20),
('Doll', 5, 25.00, 30),
('Basketball', 6, 30.00, 20),
('Conditioner', 7, 12.00, 35),
('Multivitamin', 8, 15.00, 40),
('Car Polish', 9, 22.00, 18),
('Cheese', 10, 5.00, 60),
('Smartwatch', 2, 150.00, 25),
('Jacket', 3, 55.00, 30),
('Air Fryer', 4, 90.00, 12),
('Puzzle', 5, 18.00, 20),
('Sneakers', 6, 70.00, 22),
('Cologne', 7, 65.00, 15),
('Fish Oil', 8, 20.00, 30),
('Car Vacuum', 9, 50.00, 10),
('Yogurt', 10, 2.50, 80),
('E-Reader', 2, 100.00, 18),
('Sweater', 3, 40.00, 35),
('Microwave', 4, 80.00, 15),
('LEGO Set', 5, 50.00, 25),
('Tennis Racket', 6, 45.00, 20),
('Face Cream', 7, 25.00, 30),
('Calcium Supplement', 8, 18.00, 40),
('Car Seat Cover', 9, 35.00, 12),
('Butter', 10, 4.50, 70),
('Desktop PC', 2, 900.00, 10),
('Scarf', 3, 15.00, 50);
GO

-- Chèn dữ liệu vào bảng Order và OrderDetail
DECLARE @i INT = 1;
WHILE @i <= 1000 -- Tạo 1000 đơn hàng
BEGIN
    -- Chọn ngẫu nhiên một CustomerID từ bảng Customer
    DECLARE @CustomerID INT = (SELECT TOP 1 CustomerID FROM Customer ORDER BY NEWID());
    
    -- Chọn ngẫu nhiên một EmployeeID từ bảng Employee
    DECLARE @EmployeeID INT = (SELECT TOP 1 EmployeeID FROM Employee ORDER BY NEWID());
    
    -- Tạo ngày đặt hàng ngẫu nhiên trong vòng 30 ngày qua
DECLARE @OrderDate DATETIME = DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE());

    -- Chèn dữ liệu vào bảng Order
    INSERT INTO [Order] (CustomerID, EmployeeID, OrderDate)
    VALUES (@CustomerID, @EmployeeID, @OrderDate);

    -- Lấy OrderID vừa chèn
    DECLARE @OrderID INT = SCOPE_IDENTITY();
    
    -- Số lượng sản phẩm trong đơn hàng (ngẫu nhiên từ 2 đến 5)
    DECLARE @j INT = 1;
    DECLARE @ProductCount INT = 2 + ABS(CHECKSUM(NEWID()) % 4); -- Ngẫu nhiên từ 2 đến 5 sản phẩm

    -- Chèn các sản phẩm vào bảng OrderDetail
    WHILE @j <= @ProductCount
    BEGIN
        -- Chọn ngẫu nhiên một ProductID từ bảng Product
        DECLARE @ProductID INT = (SELECT TOP 1 ProductID FROM Product ORDER BY NEWID());
        
        -- Số lượng sản phẩm (ngẫu nhiên từ 1 đến 5)
        DECLARE @Quantity INT = 1 + ABS(CHECKSUM(NEWID()) % 5);

        -- Chèn dữ liệu vào bảng OrderDetail
        INSERT INTO OrderDetail (OrderID, ProductID, Quantity)
        VALUES (@OrderID, @ProductID, @Quantity);

        SET @j = @j + 1;
    END

    SET @i = @i + 1;
END
GO

