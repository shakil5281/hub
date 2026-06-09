-- Run this script in SSMS as sysadmin (Windows Authentication).
-- Fixes: CREATE DATABASE permission denied / 502.5 startup failure under IIS.

USE [master];
GO

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'ERPHub')
BEGIN
    CREATE DATABASE [ERPHub];
    PRINT 'Database ERPHub created.';
END
ELSE
    PRINT 'Database ERPHub already exists.';
GO

-- App pool "hub" uses LocalSystem identity (from IIS Advanced Settings).
-- Grant access for both LocalSystem and the hub app pool identity.

USE [master];
GO
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'NT AUTHORITY\SYSTEM')
    CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS;
GO
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\hub')
    CREATE LOGIN [IIS APPPOOL\hub] FROM WINDOWS;
GO

USE [ERPHub];
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'NT AUTHORITY\SYSTEM')
    CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM];
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS APPPOOL\hub')
    CREATE USER [IIS APPPOOL\hub] FOR LOGIN [IIS APPPOOL\hub];
GO

ALTER ROLE [db_owner] ADD MEMBER [NT AUTHORITY\SYSTEM];
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\hub];
GO

PRINT 'SQL permissions granted. Recycle IIS app pool "hub" and test http://localhost/hub/Account/Login';
GO
