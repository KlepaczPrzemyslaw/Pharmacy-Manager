-- Database

CREATE DATABASE Pharmacy;
GO

-- Use

USE Pharmacy;

-- Baza danych 

CREATE TABLE Medicines(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name VARCHAR(256) NOT NULL UNIQUE,
	Manufacturer VARCHAR(256) NOT NULL,
	Price DECIMAL(18, 2) NOT NULL,
	Amount INT NOT NULL,
	WithPrescription BIT NOT NULL,
);

CREATE TABLE Prescriptions(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	CustomerName VARCHAR(256) NOT NULL,
	PESEL BIGINT NOT NULL UNIQUE check (len(PESEL)=11),
	PrescriptionNumber BIGINT NOT NULL UNIQUE,
);

CREATE TABLE Orders(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	PrescriptionID INT NULL,
	MedicineID INT NOT NULL,
	Date DATETIME NOT NULL,
	Amount INT NOT NULL
);

-- Log

CREATE TABLE LOGMedicines(
	LogDate DATETIME NOT NULL,
	CommandType VARCHAR(7) NOT NULL,
	ID INT NULL,
	Name VARCHAR(256) NULL,
	Manufacturer VARCHAR(256) NULL,
	Price DECIMAL(18, 2) NULL,
	Amount INT NULL,
	WithPrescription BIT NULL,
);

CREATE TABLE LOGPrescriptions(
	LogDate DATETIME NOT NULL,
	CommandType VARCHAR(7) NOT NULL,
	ID INT NULL,
	CustomerName VARCHAR(256) NULL,
	PESEL BIGINT NULL,
	PrescriptionNumber BIGINT NULL,
);

CREATE TABLE LOGOrders(
	LogDate DATETIME NOT NULL,
	CommandType VARCHAR(7) NOT NULL,
	ID INT NULL, 
	PrescriptionID INT NULL,
	MedicineID INT NULL,
	Date DATETIME NULL,
	Amount INT NULL
);

-- Klucz obcy

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Medicines FOREIGN KEY (MedicineID)
REFERENCES Medicines (ID);

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Prescriptions FOREIGN KEY (PrescriptionID)
REFERENCES Prescriptions (ID);

-- Index

CREATE NONCLUSTERED INDEX Name_Index ON Medicines (Name);
CREATE NONCLUSTERED INDEX PrescriptionNumber_Index ON Prescriptions (PrescriptionNumber);

-- Trigger
GO

CREATE TRIGGER MedicinesLogInsert
ON Medicines
AFTER INSERT
AS
	INSERT INTO LOGMedicines VALUES 
	(GETDATE(), 
	'INSERT',
	(SELECT ID FROM inserted), 
	(SELECT Name FROM inserted), 
	(SELECT Manufacturer FROM inserted), 
	(SELECT Price FROM inserted), 
	(SELECT Amount FROM inserted), 
	(SELECT WithPrescription FROM inserted))
GO

CREATE TRIGGER MedicinesLogUpdate
ON Medicines
AFTER UPDATE
AS
	INSERT INTO LOGMedicines VALUES 
	(GETDATE(), 
	'UPDATE',
	(SELECT ID FROM inserted), 
	(SELECT Name FROM inserted), 
	(SELECT Manufacturer FROM inserted), 
	(SELECT Price FROM inserted), 
	(SELECT Amount FROM inserted), 
	(SELECT WithPrescription FROM inserted))
GO

CREATE TRIGGER MedicinesLogDelete
ON Medicines
AFTER DELETE
AS
	INSERT INTO LOGMedicines (LogDate, CommandType, ID) VALUES (GETDATE(), 'DELETE', (SELECT ID FROM deleted))
GO

CREATE TRIGGER PrescriptionsLogInsert
ON Prescriptions
AFTER INSERT
AS
	INSERT INTO LOGPrescriptions VALUES 
	(GETDATE(), 
	'INSERT',
	(SELECT ID FROM inserted), 
	(SELECT CustomerName FROM inserted), 
	(SELECT PESEL FROM inserted), 
	(SELECT PrescriptionNumber FROM inserted))
GO

CREATE TRIGGER PrescriptionsLogUpdate
ON Prescriptions
AFTER UPDATE
AS
	INSERT INTO LOGPrescriptions VALUES 
	(GETDATE(), 
	'UPDATE',
	(SELECT ID FROM inserted), 
	(SELECT CustomerName FROM inserted), 
	(SELECT PESEL FROM inserted), 
	(SELECT PrescriptionNumber FROM inserted))
GO

CREATE TRIGGER PrescriptionsLogDelete
ON Prescriptions
AFTER DELETE
AS
	INSERT INTO LOGPrescriptions (LogDate, CommandType, ID) VALUES (GETDATE(), 'DELETE', (SELECT ID FROM deleted))
GO

CREATE TRIGGER OrdersLogInsert
ON Orders
AFTER INSERT
AS
	INSERT INTO LOGOrders VALUES 
	(GETDATE(), 
	'INSERT',
	(SELECT ID FROM inserted), 
	(SELECT PrescriptionID FROM inserted), 
	(SELECT MedicineID FROM inserted), 
	(SELECT Date FROM inserted), 
	(SELECT Amount FROM inserted))
GO

CREATE TRIGGER OrdersLogUpdate
ON Orders
AFTER UPDATE
AS
	INSERT INTO LOGOrders VALUES 
	(GETDATE(), 
	'UPDATE',
	(SELECT ID FROM inserted), 
	(SELECT PrescriptionID FROM inserted), 
	(SELECT MedicineID FROM inserted), 
	(SELECT Date FROM inserted), 
	(SELECT Amount FROM inserted))
GO

CREATE TRIGGER OrdersLogDelete
ON Orders
AFTER DELETE
AS
	INSERT INTO LOGOrders (LogDate, CommandType, ID) VALUES (GETDATE(), 'DELETE', (SELECT ID FROM deleted))
GO