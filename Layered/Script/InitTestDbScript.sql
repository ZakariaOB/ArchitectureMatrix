-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MachineMonitoringTest')
BEGIN
    CREATE DATABASE MachineMonitoringTest;
END
GO

USE MachineMonitoringTest
GO

IF OBJECT_ID('Machine', 'U') IS NULL
BEGIN
    CREATE TABLE Machine (
        machineId INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
        Name VARCHAR(50) NOT NULL,
        Description VARCHAR(250)
    );
END

IF OBJECT_ID('MachineProduction', 'U') IS NULL
BEGIN
    CREATE TABLE MachineProduction (
        MachineProductionId INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
        machineId INT NOT NULL,
        totalProduction INT NOT NULL DEFAULT 0,
        FOREIGN KEY (machineId) REFERENCES Machine(machineId)
    );
END

DECLARE @MachineData TABLE (
  Name VARCHAR(50) NOT NULL,
  Description VARCHAR(250),
  totalProduction INT
);

-- Using this table to avoid duplications after running the script multiple times
INSERT INTO @MachineData (Name, Description, totalProduction)
VALUES
  ('Machine 1', 'Machine 1 Description', 700),
  ('Machine 1', 'Machine 1 Description', 300),
  ('Machine 2', 'Machine 2 Description', 1500),
  ('Machine 2', 'Machine 2 Description', 500)

DECLARE @Name VARCHAR(50); 
DECLARE @Description VARCHAR(250); 
DECLARE @TotalProduction INT;
DECLARE @MachineId INT;

DECLARE MachineCursor CURSOR FOR
SELECT Name, Description, totalProduction
FROM @MachineData;

OPEN MachineCursor;
FETCH NEXT FROM MachineCursor INTO @Name, @Description, @TotalProduction;

WHILE @@FETCH_STATUS = 0
BEGIN
  IF NOT EXISTS (SELECT * FROM Machine WHERE Name = @Name)
  BEGIN
    INSERT INTO Machine (Name, Description)
    VALUES (@Name, @Description);
  END;
  
  SET @MachineId = (SELECT machineId FROM Machine WHERE Name = @Name);

  IF NOT EXISTS (SELECT * FROM MachineProduction WHERE machineId = @MachineId)
  BEGIN
    INSERT INTO MachineProduction (machineId, totalProduction)
    VALUES (@MachineId, @TotalProduction);
  END
  ELSE 
  BEGIN
	INSERT INTO MachineProduction (machineId, totalProduction)
    VALUES (@MachineId, @TotalProduction);
  END;

  FETCH NEXT FROM MachineCursor INTO @Name, @Description, @TotalProduction;
END;

CLOSE MachineCursor;
DEALLOCATE MachineCursor;
