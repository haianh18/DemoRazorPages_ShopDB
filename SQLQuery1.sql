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
('John Doe', 'Manager'),
('Jane Smith', 'Sales Associate'),
('Alice Johnson', 'Cashier'),
('Bob Brown', 'Warehouse Staff'),
('Charlie Davis', 'IT Support'),
('Eva Green', 'Marketing Specialist'),
('Frank White', 'HR Manager'),
('Grace Lee', 'Accountant'),
('Henry Wilson', 'Security Guard'),
('Ivy Taylor', 'Customer Service');
GO

-- Chèn dữ liệu vào bảng Customer (không tuân theo thứ tự)
INSERT INTO Customer (CustomerName, Email, Phone) VALUES
('Michael Scott', 'michael@example.com', '123-456-7890'),
('Pam Beesly', 'pam@example.com', '234-567-8901'),
('Jim Halpert', 'jim@example.com', '345-678-9012'),
('Dwight Schrute', 'dwight@example.com', '456-789-0123'),
('Angela Martin', 'angela@example.com', '567-890-1234'),
('Kevin Malone', 'kevin@example.com', '678-901-2345'),
('Andy Bernard', 'andy@example.com', '789-012-3456'),
('Stanley Hudson', 'stanley@example.com', '890-123-4567'),
('Phyllis Vance', 'phyllis@example.com', '901-234-5678'),
('Oscar Martinez', 'oscar@example.com', '012-345-6789');
GO

-- Chèn dữ liệu vào bảng Product (không tuân theo thứ tự)
INSERT INTO Product (ProductName, CategoryID, Price, Quantity) VALUES
('Laptop', 2, 1200.00, 10),
('T-Shirt', 3, 20.00, 50),
('Novel', 1, 15.00, 30),
('Blender', 4, 50.00, 15),
('Action Figure', 5, 30.00, 20),
('Football', 6, 25.00, 25),
('Shampoo', 7, 10.00, 40),
('Vitamin C', 8, 12.00, 35),
('Car Wax', 9, 18.00, 12),
('Bread', 10, 3.00, 100),
('Smartphone', 2, 800.00, 20),
('Jeans', 3, 45.00, 30),
('Cookware Set', 4, 120.00, 10),
('Board Game', 5, 35.00, 15),
('Running Shoes', 6, 60.00, 25),
('Perfume', 7, 70.00, 20),
('Protein Powder', 8, 25.00, 30),
('Tire Pump', 9, 40.00, 10),
('Milk', 10, 4.00, 50);
GO

-- Chèn dữ liệu vào bảng Order và OrderDetail
DECLARE @i INT = 1;
WHILE @i <= 15 -- Tạo 15 đơn hàng
BEGIN
    DECLARE @CustomerID INT = (SELECT TOP 1 CustomerID FROM Customer ORDER BY NEWID());
    DECLARE @EmployeeID INT = (SELECT TOP 1 EmployeeID FROM Employee ORDER BY NEWID());
    DECLARE @OrderDate DATETIME = DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE()); -- Ngẫu nhiên trong 30 ngày qua

    INSERT INTO [Order] (CustomerID, EmployeeID, OrderDate)
    VALUES (@CustomerID, @EmployeeID, @OrderDate);

    DECLARE @OrderID INT = SCOPE_IDENTITY();
    DECLARE @j INT = 1;
    DECLARE @ProductCount INT = 2 + ABS(CHECKSUM(NEWID()) % 4); -- Ngẫu nhiên từ 2 đến 5 sản phẩm

    WHILE @j <= @ProductCount
    BEGIN
        DECLARE @ProductID INT = (SELECT TOP 1 ProductID FROM Product ORDER BY NEWID());
        DECLARE @Quantity INT = 1 + ABS(CHECKSUM(NEWID()) % 5); -- Ngẫu nhiên số lượng từ 1 đến 5

        INSERT INTO OrderDetail (OrderID, ProductID, Quantity)
        VALUES (@OrderID, @ProductID, @Quantity);

        SET @j = @j + 1;
    END

    SET @i = @i + 1;
END
GO

