-- Database initialization script for LabScheduler
-- This runs automatically when SQL Server container starts

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LabScheduler')
BEGIN
    CREATE DATABASE LabScheduler;
END
GO

USE LabScheduler;
GO
