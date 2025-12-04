-- 创建数据库
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DMS')
BEGIN
    CREATE DATABASE DMS;
END
GO

USE DMS;
GO

-- 创建 Schema: DMS
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'DMS')
BEGIN
    EXEC('CREATE SCHEMA DMS');
END
GO

-- 创建 Schema: RIS
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'RIS')
BEGIN
    EXEC('CREATE SCHEMA RIS');
END
GO

-- 设置数据库为多用户模式
ALTER DATABASE DMS SET MULTI_USER;
GO

PRINT 'Database DMS and schemas DMS, RIS created successfully!';
GO


