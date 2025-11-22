-- Users (for login/authentication)
CREATE TABLE Users (
    UserID INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    Role NVARCHAR(20) NOT NULL, -- Admin / Staff
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Vendors
CREATE TABLE Vendors (
    VendorID INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100),
    ContactNo NVARCHAR(20),
    CompanyName NVARCHAR(100),
    Address NVARCHAR(200),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Vendor Ledgers
CREATE TABLE VendorLedgers (
    LedgerID INT IDENTITY PRIMARY KEY,
    VendorID INT FOREIGN KEY REFERENCES Vendors(VendorID),
    Date DATETIME DEFAULT GETDATE(),
    TransactionType NVARCHAR(50), -- Purchase / Payment / Return
    DebitAmount DECIMAL(18,2) DEFAULT 0,
    CreditAmount DECIMAL(18,2) DEFAULT 0,
    Balance DECIMAL(18,2) DEFAULT 0
);

-- Products
CREATE TABLE Products (
    ProductID INT IDENTITY PRIMARY KEY,
    ModelNo NVARCHAR(50) UNIQUE,
    ProductName NVARCHAR(100),
    Brand NVARCHAR(50),
    Category NVARCHAR(50),
    PurchasePrice DECIMAL(18,2),
    SalePrice DECIMAL(18,2),
    QuantityInStock INT DEFAULT 0,
    ReorderLevel INT DEFAULT 5,
    Name varchar(100) NULL
);

-- Company Purchases
CREATE TABLE CompanyPurchase (
    PurchaseID INT IDENTITY PRIMARY KEY,
    VendorID INT FOREIGN KEY REFERENCES Vendors(VendorID),
    PurchaseDate DATETIME DEFAULT GETDATE(),
    InvoiceNo NVARCHAR(50),
    TotalAmount DECIMAL(18,2),
    PaidAmount DECIMAL(18,2),
    RemainingAmount AS (TotalAmount - PaidAmount) PERSISTED,
    PaymentStatus NVARCHAR(20) -- Paid / Partial / Pending
);

-- Purchase Details
CREATE TABLE PurchaseDetails (
    PurchaseDetailID INT IDENTITY PRIMARY KEY,
    PurchaseID INT FOREIGN KEY REFERENCES CompanyPurchase(PurchaseID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT,
    UnitPrice DECIMAL(18,2),
    TotalPrice AS (Quantity * UnitPrice) PERSISTED
);

-- Customers
CREATE TABLE Customers (
    CustomerID INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100),
    ContactNo NVARCHAR(20),
    Address NVARCHAR(200)
);

-- Customer Credits
CREATE TABLE CustomerCredits (
    CreditID INT IDENTITY PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    Date DATETIME DEFAULT GETDATE(),
    InvoiceNo NVARCHAR(50),
    TotalAmount DECIMAL(18,2),
    PaidAmount DECIMAL(18,2),
    RemainingAmount AS (TotalAmount - PaidAmount) PERSISTED
);

-- Warehouse
CREATE TABLE Warehouse (
    WarehouseID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
    Manager NVARCHAR(100) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);


-- Warehouse In/Out
CREATE TABLE WarehouseInOut (
    TransactionID INT IDENTITY PRIMARY KEY,
    WarehouseID INT FOREIGN KEY REFERENCES Warehouse(WarehouseID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    TransactionType NVARCHAR(10), -- In / Out
    Quantity INT,
    Date DATETIME DEFAULT GETDATE(),
    ReferenceNo NVARCHAR(50) -- PurchaseID or SaleID
);

ALTER TABLE WarehouseInOut
ADD PurchaseID INT NULL;

ALTER TABLE WarehouseInOut
ADD CONSTRAINT FK_WarehouseInOut_Purchase
FOREIGN KEY (PurchaseID) REFERENCES CompanyPurchase(PurchaseID);

-- Staff
CREATE TABLE Staff (
    StaffID INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100),
    Role NVARCHAR(50),
    Salary DECIMAL(18,2),
    JoiningDate DATETIME DEFAULT GETDATE(),
    ContactNo NVARCHAR(20),
    Address NVARCHAR(200)
);

-- Utilities
CREATE TABLE Utilities (
    UtilityID INT IDENTITY PRIMARY KEY,
    UtilityType NVARCHAR(50),
    Month NVARCHAR(20),
    Amount DECIMAL(18,2),
    PaidStatus NVARCHAR(20)
);

-- Sales (Invoice header)
CREATE TABLE Sales (
    SaleID INT IDENTITY PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    SaleDate DATETIME DEFAULT GETDATE(),
    InvoiceNo NVARCHAR(50),
    TotalAmount DECIMAL(18,2),
    PaidAmount DECIMAL(18,2),
    RemainingAmount AS (TotalAmount - PaidAmount) PERSISTED,
    PaymentStatus NVARCHAR(20) -- Paid / Partial / Pending
);

ALTER TABLE WarehouseInOut
ADD SaleID INT NULL

ALTER TABLE WarehouseInOut 
ADD CONSTRAINT FK_WarehouseInOut_Sale FOREIGN KEY (SaleID) REFERENCES Sales(SaleID);

-- Sales Details (Invoice line items)
CREATE TABLE SalesDetails (
    SaleDetailID INT IDENTITY PRIMARY KEY,
    SaleID INT FOREIGN KEY REFERENCES Sales(SaleID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT,
    UnitPrice DECIMAL(18,2), -- Selling price per unit
    CostPrice DECIMAL(18,2), -- From Products.PurchasePrice (to track profit/loss)
    Profit AS ((UnitPrice - CostPrice) * Quantity) PERSISTED
);







select * from Vendors
SELECT * FROM VendorLedgers
SELECT * FROM Products
SELECT * FROM Warehouse
SELECT * FROM PurchaseDetails
SELECT * FROM VendorLedgers
SELECT * FROM WarehouseInOut
select * from Customers
select * from Sales
select * from CompanyPurchase
select * from Users

SELECT * FROM Products
SELECT * FROM CompanyPurchase
SELECT * FROM PurchaseDetails

drop table Warehouse
ALTER TABLE Products drop COLUMN WarehouseID
ALTER TABLE Products ADD Name varchar(100) null

update Products 
set Name = 'NEW'
WHERE ModelNo = 'L-1'



-- 1. Declare table variable of type TVP_SaleDetails
DECLARE @SaleItems TVP_SaleDetails;

-- 2. Insert products being sold
INSERT INTO @SaleItems (ProductID, Quantity, UnitPrice)
VALUES 
(1, 2, 1500.00),   -- Selling 2 units of ProductID 1 at 1500 each
(3, 1, 2500.00);   -- Selling 1 unit of ProductID 3 at 2500

-- 3. Call the procedure
EXEC sp_CreateSale 
    @CustomerID = 1,
    @InvoiceNo = 'SALE-1001',
    @PaidAmount = 3000.00,
    @PaymentStatus = 'Partial',
    @WarehouseID = 2,
    @Products = @SaleItems;


	SELECT  CustomerID, Name, ContactNo, Address, CreatedDate FROM Customers


	select * from WarehouseInOut

	SELECT p.ProductID, p.Name AS Products, w.Name AS Warehouse, wi.Date, wi.Quantity from Products p
	inner join WarehouseInOut wi ON
	p.ProductID = wi.ProductID 
	inner join Warehouse W ON
		wi.WarehouseID = W.WarehouseID
		where TransactionType = 'In' AND w.Name = 'Shop'


SELECT 
    p.ProductID,
    p.ProductName,
    w.Name AS Warehouse,
    SUM(CASE WHEN wi.TransactionType = 'In' THEN wi.Quantity ELSE 0 END) AS TotalIn,
    SUM(CASE WHEN wi.TransactionType = 'Out' THEN wi.Quantity ELSE 0 END) AS TotalOut,
    SUM(CASE WHEN wi.TransactionType = 'In' THEN wi.Quantity ELSE 0 END) -
    SUM(CASE WHEN wi.TransactionType = 'Out' THEN wi.Quantity ELSE 0 END) AS CurrentStock
FROM Products p
INNER JOIN WarehouseInOut wi ON p.ProductID = wi.ProductID
INNER JOIN Warehouse w ON wi.WarehouseID = w.WarehouseID
WHERE w.Name = 'Shop'
GROUP BY p.ProductID, p.ProductName, w.Name;

SELECT 
    p.ProductID,
    p.ProductName,
    w.Name AS Warehouse,
    wi.TransactionType,
    wi.Quantity,
    wi.Date
FROM Products p
INNER JOIN WarehouseInOut wi ON p.ProductID = wi.ProductID
INNER JOIN Warehouse w ON wi.WarehouseID = w.WarehouseID
WHERE w.Name = 'Shop'
ORDER BY wi.Date DESC;





DELETE VendorLedgers
DELETE Vendors
DELETE CompanyPurchase
DELETE PurchaseDetails
DELETE WarehouseInOut
DELETE Warehouse
DELETE Products
DELETE Sales
DELETE SalesDetails

SELECT v.LedgerID, v.VendorID, Ve.Name, v.Date, v.TransactionType, v.DebitAmount, v.CreditAmount, v.Balance
FROM VendorLedgers v 
INNER JOIN Vendors Ve ON
v.VendorID = Ve.VendorID












-- Users, Roles, Permissions, RolePermissions
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(Id)
);

CREATE TABLE Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL UNIQUE,
    Route NVARCHAR(250) NULL
);

CREATE TABLE RolePermissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(Id),
    PermissionId INT NOT NULL FOREIGN KEY REFERENCES Permissions(Id),
    IsAllowed BIT NOT NULL DEFAULT 0,
    CONSTRAINT UQ_RolePermission UNIQUE (RoleId, PermissionId)
);

-- Seed some sample values
INSERT INTO Roles (Name) VALUES ('Admin'), ('Cashier'), ('Salesman');
INSERT INTO Permissions (Name, Route) VALUES
('View Dashboard', '/dashboard'),
('View Sales Report', '/sales-report'),
('View Summary Report', '/summary-report');

INSERT INTO Permissions (Name, Route) VALUES
('View Stock Movement', '/stock-movement'),

-- give Admin all permissions (1 = Admin role id may differ)
INSERT INTO RolePermissions (RoleId, PermissionId, IsAllowed)
SELECT r.Id, p.Id, 1
FROM Roles r CROSS JOIN Permissions p
WHERE r.Name = 'Salesman';

-- Cashier initially cannot view summary
INSERT INTO RolePermissions (RoleId, PermissionId, IsAllowed)
SELECT r.Id, p.Id, CASE WHEN p.Name = 'View Summary Report' THEN 0 ELSE 1 END
FROM Roles r CROSS JOIN Permissions p
WHERE r.Name = 'Cashier';


select * from Roles
select * from Users
select * from Permissions
select * from RolePermissions

UPDATE Users
SET Role = 'Salesman'
WHERE UserID = 3

DROP TABLE Users

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleId INT FOREIGN KEY REFERENCES Roles(Id),
	    CreatedDate DATETIME DEFAULT GETDATE()

);