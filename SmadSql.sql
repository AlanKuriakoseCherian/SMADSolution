Create Database SmadDb;
 
Use SmadDb;
 
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME
);
 
INSERT INTO Users (Username, PasswordHash, Role, LastLogin)
VALUES 
('admin', 'password123', 'Admin', '2022-01-01 12:00:00'),
('operator1', 'password456', 'Operator', '2022-01-02 10:00:00'),
('manager1', 'password789', 'Manager', '2022-01-03 14:00:00'),
('user1', 'password101', 'User ', '2022-01-04 16:00:00'),
('user2', 'password202', 'User ', '2022-01-05 18:00:00'),
('operator2', 'password303', 'Operator', '2022-01-06 20:00:00'),
('manager2', 'password404', 'Manager', '2022-01-07 22:00:00'),
('admin2', 'password505', 'Admin', '2022-01-08 00:00:00'),
('user3', 'password606', 'User ', '2022-01-09 02:00:00'),
('user4', 'password707', 'User ', '2022-01-10 04:00:00');
 
Select * From Users;
 
CREATE TABLE ProductionLines (
    LineID INT PRIMARY KEY IDENTITY(1,1),
    LineName NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL,  -- e.g., Active, Inactive
    CreatedAt DATETIME DEFAULT GETDATE()
);
 
INSERT INTO ProductionLines (LineName, Status)
VALUES 
('Line 1', 'Active'),
('Line 2', 'Active'),
('Line 3', 'Active'),
('Line 4', 'Inactive'),
('Line 5', 'Active'),
('Line 6', 'Inactive'),
('Line 7', 'Active'),
('Line 8', 'Active'),
('Line 9', 'Inactive'),
('Line 10', 'Active');
 
Select * From ProductionLines;
 
CREATE TABLE ProductionMetrics (
    MetricID INT PRIMARY KEY IDENTITY(1,1),
    LineID INT NOT NULL,
    MetricDate DATETIME NOT NULL,
    ProductionRate DECIMAL(18, 2),  -- Units produced per hour
    Efficiency DECIMAL(5, 2),       -- Percentage (0-100)
    QualityRate DECIMAL(5, 2),       -- Percentage (0-100)
    Downtime DECIMAL(18, 2),         -- Hours of downtime
    FOREIGN KEY (LineID) REFERENCES ProductionLines(LineID)
);
 
INSERT INTO ProductionMetrics (LineID, MetricDate, ProductionRate, Efficiency, QualityRate, Downtime)
VALUES 
(1, '2022-01-01 08:00:00', 100.00, 90.00, 95.00, 0.50),
(1, '2022-01-01 12:00:00', 120.00, 92.00, 96.00, 0.25),
(1, '2022-01-01 16:00:00', 110.00, 91.00, 94.00, 0.75),
(2, '2022-01-01 08:00:00', 90.00, 88.00, 92.00, 1.00),
(2, '2022-01-01 12:00:00', 100.00, 90.00, 93.00, 0.50),
(2, '2022-01-01 16:00:00', 95.00, 89.00, 91.00, 1.25),
(3, '2022-01-01 08:00:00', 105.00, 93.00, 97.00, 0.25),
(3, '2022-01-01 12:00:00', 115.00, 94.00, 98.00, 0.50),
(3, '2022-01-01 16:00:00', 100.00, 92.00, 95.00, 1.00),
(1, '2022-01-02 08:00:00', 110.00, 91.00, 94.00, 0.75),
(1, '2022-01-02 12:00:00', 125.00, 93.00, 96.00, 0.25),
(1, '2022-01-02 16:00:00', 105.00, 90.00, 93.00, 1.00),
(2, '2022-01-02 08:00:00', 95.00, 89.00, 91.00, 1.25),
(2, '2022-01-02 12:00:00', 105.00, 91.00, 93.00, 0.50),
(2, '2022-01-02 16:00:00', 90.00, 88.00, 90.00, 1.50),
(3, '2022-01-02 08:00:00', 115.00, 94.00, 98.00, 0.50),
(3, '2022-01-02 12:00:00', 120.00, 95.00, 99.00, 0.25),
(3, '2022-01-02 16:00:00', 110.00, 93.00, 96.00, 0.75);
 
Select * From ProductionMetrics;
 
CREATE TABLE Alerts (
    AlertID INT PRIMARY KEY IDENTITY(1,1),
    LineID INT NOT NULL,
    AlertDate DATETIME DEFAULT GETDATE(),
    AlertType NVARCHAR(100) NOT NULL,  -- e.g., Machine Failure, Quality Issue
    Severity NVARCHAR(50),              -- e.g., Low, Medium, High
    Message NVARCHAR(255) NOT NULL,
    Resolved BIT DEFAULT 0,
    FOREIGN KEY (LineID) REFERENCES ProductionLines(LineID)
);
 
INSERT INTO Alerts (LineID, AlertDate, AlertType, Severity, Message, Resolved)
VALUES 
(1, '2022-01-01 10:00:00', 'Machine Failure', 'High', 'Machine 1 has stopped working', 0),
(1, '2022-01-01 14:00:00', 'Quality Issue', 'Medium', 'Product quality is below standard', 0),
(2, '2022-01-02 08:00:00', 'Machine Failure', 'High', 'Machine 2 has broken down', 0),
(2, '2022-01-02 12:00:00', 'Quality Issue', 'Low', 'Product quality is slightly below standard', 0),
(3, '2022-01-01 16:00:00', 'Machine Failure', 'High', 'Machine 3 has malfunctioned', 0),
(3, '2022-01-02 10:00:00', 'Quality Issue', 'Medium', 'Product quality is inconsistent', 0),
(1, '2022-01-03 08:00:00', 'Machine Failure', 'High', 'Machine 1 has stopped working again', 0),
(2, '2022-01-03 12:00:00', 'Quality Issue', 'Low', 'Product quality is slightly below standard again', 0),
(3, '2022-01-03 16:00:00', 'Machine Failure', 'High', 'Machine 3 has broken down again', 0);
 
Select * From Alerts;
 
CREATE TABLE Settings (
    SettingID INT PRIMARY KEY IDENTITY(1,1),
    SettingKey NVARCHAR(100) NOT NULL UNIQUE,
    SettingValue NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);
 
INSERT INTO Settings (SettingKey, SettingValue)
VALUES 
('ProductionRateThreshold', '80'),
('EfficiencyThreshold', '85'),
('QualityRateThreshold', '90'),
('DowntimeThreshold', '2'),
('AlertEmail', 'alerts@example.com'),
('AlertPhoneNumber', '+1234567890'),
('MachineFailureAlertType', 'Machine Failure'),
('QualityIssueAlertType', 'Quality Issue'),
('LowSeverity', 'Low'),
('MediumSeverity', 'Medium'),
('HighSeverity', 'High');
 
Select * From Settings;
 
CREATE TABLE ProductionTrends (
    TrendID INT PRIMARY KEY IDENTITY(1,1),
    LineID INT NOT NULL,
    TrendDate DATETIME NOT NULL,
    AverageProductionRate DECIMAL(18, 2),
    AverageEfficiency DECIMAL(5, 2),
    AverageQualityRate DECIMAL(5, 2),
    FOREIGN KEY (LineID) REFERENCES ProductionLines(LineID)
);
 
INSERT INTO ProductionTrends (LineID, TrendDate, AverageProductionRate, AverageEfficiency, AverageQualityRate)
VALUES 
(1, '2022-01-01 00:00:00', 90.00, 92.00, 95.00),
(1, '2022-01-02 00:00:00', 88.00, 90.00, 93.00),
(1, '2022-01-03 00:00:00', 85.00, 88.00, 90.00),
(2, '2022-01-01 00:00:00', 92.00, 94.00, 96.00),
(2, '2022-01-02 00:00:00', 90.00, 92.00, 94.00),
(2, '2022-01-03 00:00:00', 88.00, 90.00, 92.00),
(3, '2022-01-01 00:00:00', 95.00, 97.00, 98.00),
(3, '2022-01-02 00:00:00', 93.00, 95.00, 96.00),
(3, '2022-01-03 00:00:00', 90.00, 92.00, 94.00),
(1, '2022-01-04 00:00:00', 80.00, 85.00, 90.00),
(2, '2022-01-04 00:00:00', 85.00, 88.00, 92.00),
(3, '2022-01-04 00:00:00', 88.00, 90.00, 94.00);
 
Select * From ProductionTrends;