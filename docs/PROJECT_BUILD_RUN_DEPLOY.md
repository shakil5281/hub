# ERP Hub — Build, Run, Migrations & IIS Deployment Guide

This guide explains how to build and run the ERP Hub project locally, manage Entity Framework Core migrations, and publish the application to a local IIS server.

---

## Table of Contents

1. [Prerequisites](#1-prerequisites)
2. [Project Structure](#2-project-structure)
3. [Build and Run (Development)](#3-build-and-run-development)
4. [Database Migrations](#4-database-migrations)
5. [Publish to Local IIS](#5-publish-to-local-iis)
6. [Troubleshooting](#6-troubleshooting)

---

## 1. Prerequisites

| Requirement | Version / Notes |
|-------------|-----------------|
| **.NET SDK** | .NET 10 (`dotnet --version` → e.g. `10.0.103`) |
| **Node.js** | 18 or later (for Tailwind CSS build) |
| **SQL Server** | SQL Server Express (`localhost\SQLEXPRESS`) or LocalDB |
| **EF Core CLI** | `dotnet tool install --global dotnet-ef` |
| **IIS (for publish)** | Windows IIS + **ASP.NET Core Hosting Bundle for .NET 10** |

### Verify installations

```powershell
dotnet --version
node --version
dotnet ef --version
```

### Install EF Core tools (if missing)

```powershell
dotnet tool install --global dotnet-ef
```

---

## 2. Project Structure

```
hrhub/
├── docs/                          # Documentation
├── ERP.Web/                       # Main ASP.NET Core MVC application
│   ├── Areas/                     # Feature modules (HR, Payroll, Leave, etc.)
│   ├── Core/                      # Entities, interfaces, enums
│   ├── Infrastructure/            # Data, services, repositories
│   │   └── Data/
│   │       ├── AppDbContext.cs
│   │       ├── AppDbContextFactory.cs
│   │       └── Migrations/        # EF Core migration files
│   ├── wwwroot/                   # Static files (CSS, JS)
│   ├── appsettings.json           # Connection string & config
│   ├── ERP.Web.csproj
│   └── package.json               # Tailwind CSS build scripts
└── publish/                       # Output folder after dotnet publish (generated)
```

| Setting | Value |
|---------|-------|
| **Framework** | `net10.0` |
| **Database** | SQL Server via EF Core |
| **DbContext** | `AppDbContext` |
| **Dev URLs** | `http://localhost:5080` / `https://localhost:5081` |
| **Default login** | `admin@erp.com` / `Admin@123` |

---

## 3. Build and Run (Development)

All commands are run from the `ERP.Web` folder.

### Step 1 — Install Node dependencies (first time only)

```powershell
cd g:\softwer\hrhub\ERP.Web
npm install
```

### Step 2 — Build Tailwind CSS

The UI uses Tailwind CSS. Compile it before building or running the app:

```powershell
npm run build:css
```

This reads `wwwroot/css/tailwind.input.css` and outputs minified `wwwroot/css/site.css`.

### Step 3 — Build the .NET project

```powershell
dotnet build
```

### Step 4 — Apply database migrations

```powershell
dotnet ef database update
```

> **Note:** The app also applies pending migrations automatically on startup via `DbSeeder.SeedAsync()` → `context.Database.MigrateAsync()`. Running `dotnet ef database update` manually is still recommended before first run.

### Step 5 — Run the application

**HTTPS (recommended):**

```powershell
dotnet run --launch-profile https
```

**HTTP only:**

```powershell
dotnet run --launch-profile http
```

### Step 6 — Open in browser

| Profile | URL |
|---------|-----|
| HTTPS | https://localhost:5081 |
| HTTP | http://localhost:5080 |

Login page: `/Account/Login`

### Full bootstrap (one-shot)

```powershell
cd g:\softwer\hrhub\ERP.Web
npm run build:css
dotnet build
dotnet ef database update
dotnet run --launch-profile https
```

### Connection string

Edit `ERP.Web/appsettings.json` (or `appsettings.Development.json`) if your SQL Server instance differs:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ERPHub;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**LocalDB alternative:**

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ERPHub;Trusted_Connection=True;TrustServerCertificate=True"
```

---

## 4. Database Migrations

### How migrations work in this project

- Migration files live in `ERP.Web/Infrastructure/Data/Migrations/`
- Design-time factory: `AppDbContextFactory.cs` (reads `appsettings.json`)
- On app startup, `DbSeeder` calls `MigrateAsync()` to apply any pending migrations
- SQL Server database name: **ERPHub**

### Existing migrations

| Migration | Purpose |
|-----------|---------|
| `InitialCreate` | Core schema, Identity, permissions |
| `Phase2Modules` | Section, Line, LeaveBalance, BillRateConfig, organogram |
| `AdvancedFilters` | Saved filter presets |
| `EmployeeAddress` | Employee address fields |
| `EmployeeProfileExtended` | Extended employee profile |
| `DesignationSectionRelation` | Designation–Section relationship |
| `Phase3Features` | Holidays, Eid bonuses, job cards, temporary shifts |

### List applied and pending migrations

```powershell
cd g:\softwer\hrhub\ERP.Web
dotnet ef migrations list
```

### Apply all pending migrations to the database

```powershell
dotnet ef database update
```

### Apply up to a specific migration

```powershell
dotnet ef database update 20260608050804_Phase2Modules
```

### Create a new migration (after entity/model changes)

1. Update entities in `ERP.Web/Core/Entities/` and/or configurations in `ERP.Web/Infrastructure/Data/Configurations/`
2. Register new `DbSet<>` entries in `AppDbContext.cs` if needed
3. Create the migration:

```powershell
cd g:\softwer\hrhub\ERP.Web
dotnet ef migrations add YourMigrationName
```

4. Review the generated files under `Infrastructure/Data/Migrations/`
5. Apply to the database:

```powershell
dotnet ef database update
```

### Remove the last migration (only if not applied to production)

```powershell
dotnet ef migrations remove
```

### Generate SQL script (for manual DBA review)

```powershell
# All migrations
dotnet ef migrations script -o migration.sql

# From a specific migration to latest
dotnet ef migrations script 20260608044337_InitialCreate -o update.sql
```

### Roll back database to a previous migration

```powershell
dotnet ef database update PreviousMigrationName
```

### Drop and recreate database (development only)

```powershell
dotnet ef database drop --force
dotnet ef database update
```

---

## 5. Publish to Local IIS

### 5.1 — Install IIS prerequisites

1. **Enable IIS** (Windows Features):
   - Internet Information Services
   - World Wide Web Services → Application Development Features → **WebSocket Protocol** (optional)
   - **IIS Management Console**

2. **Install ASP.NET Core Hosting Bundle (.NET 10)**  
   Download from: https://dotnet.microsoft.com/download/dotnet/10.0  
   Install **Hosting Bundle** (includes .NET Runtime + ASP.NET Core Module for IIS).

3. **Restart IIS** after installing the hosting bundle:

```powershell
net stop was /y
net start w3svc
```

### 5.2 — Build and publish the application

Run from `ERP.Web`:

```powershell
cd g:\softwer\hrhub\ERP.Web

# 1. Build frontend CSS
npm run build:css

# 2. Publish Release build
dotnet publish -c Release -o g:\softwer\hrhub\publish
```

The publish folder will contain:

- `ERP.Web.dll` — main application assembly
- `web.config` — IIS configuration (auto-generated)
- `appsettings.json` — configuration (update for production)
- `wwwroot/` — static assets

### 5.3 — Configure production settings

Edit `g:\softwer\hrhub\publish\appsettings.json` with the production SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ERPHub;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Ensure the SQL Server instance is running and the `ERPHub` database exists (or will be created on first run when migrations apply).

### 5.4 — Create IIS Application Pool

1. Open **IIS Manager** (`inetmgr`)
2. Right-click **Application Pools** → **Add Application Pool**
3. Settings:
   - **Name:** `ERPHub`
   - **.NET CLR version:** `No Managed Code`
   - **Managed pipeline mode:** `Integrated`
4. Click **OK**
5. Select the `ERPHub` pool → **Advanced Settings**:
   - **Identity:** `ApplicationPoolIdentity` (default) or a dedicated Windows account with SQL Server access

### 5.5 — Create IIS Website

1. Right-click **Sites** → **Add Website**
2. Settings:
   - **Site name:** `ERPHub`
   - **Application pool:** `ERPHub`
   - **Physical path:** `g:\softwer\hrhub\publish`
   - **Binding:** e.g. `http`, port `8080`, host name blank (or `localhost`)
3. Click **OK**

### 5.6 — Set environment variable (Production)

For the IIS site, set `ASPNETCORE_ENVIRONMENT=Production`:

**Option A — web.config** (in publish folder):

```xml
<aspNetCore processPath="dotnet"
            arguments=".\ERP.Web.dll"
            stdoutLogEnabled="true"
            stdoutLogFile=".\logs\stdout"
            hostingModel="inprocess">
  <environmentVariables>
    <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
  </environmentVariables>
</aspNetCore>
```

**Option B — IIS Manager:**

1. Select the site → **Configuration Editor**
2. Section: `system.webServer/aspNetCore`
3. Add environment variable `ASPNETCORE_ENVIRONMENT` = `Production`

### 5.7 — Folder permissions

Grant the app pool identity read/execute access to the publish folder:

```powershell
icacls "g:\softwer\hrhub\publish" /grant "IIS AppPool\ERPHub:(OI)(CI)RX" /T
```

Create a `logs` folder for stdout logging (if enabled):

```powershell
mkdir g:\softwer\hrhub\publish\logs
icacls "g:\softwer\hrhub\publish\logs" /grant "IIS AppPool\ERPHub:(OI)(CI)F" /T
```

### 5.8 — Start and test

1. In IIS Manager, select the **ERPHub** site → **Start**
2. Browse to: `http://localhost:8080/Account/Login`
3. Log in with `admin@erp.com` / `Admin@123`

On first request, the app will run migrations and seed default data automatically.

### 5.9 — Re-publish after code changes

```powershell
cd g:\softwer\hrhub\ERP.Web
npm run build:css
dotnet publish -c Release -o g:\softwer\hrhub\publish
```

Then restart the IIS site (or app pool):

```powershell
# Restart app pool
Restart-WebAppPool -Name "ERPHub"
```

> **Tip:** Back up `publish\appsettings.json` before re-publishing if you changed production settings, since publish may overwrite it.

### 5.10 — Publish with a specific configuration (optional)

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -o g:\softwer\hrhub\publish
```

Use `--self-contained true` only if the server does not have the .NET 10 runtime installed (not typical when Hosting Bundle is installed).

---

## 6. Troubleshooting

| Problem | Solution |
|---------|----------|
| **Build fails — file locked** | Stop the running app (`Ctrl+C` in terminal) or stop the IIS site, then rebuild |
| **CSS looks broken** | Run `npm run build:css` before `dotnet build` or `dotnet publish` |
| **Cannot connect to SQL Server** | Verify SQL Server Express is running; check connection string in `appsettings.json` |
| **EF tools version warning** | Update: `dotnet tool update --global dotnet-ef` |
| **IIS 500.30 — app failed to start** | Install/repair ASP.NET Core Hosting Bundle; check `publish\logs\stdout_*.log` |
| **IIS 500.31 — failed to load ASP.NET Core Module** | Reinstall Hosting Bundle and restart IIS |
| **Access denied on publish folder** | Run `icacls` permission commands in section 5.7 |
| **Login works in dev but not IIS** | Confirm `ASPNETCORE_ENVIRONMENT` and connection string; ensure migrations ran (check `__EFMigrationsHistory` table) |
| **HTTPS redirect issues on IIS** | Production uses `UseHttpsRedirection()` — configure an HTTPS binding in IIS or adjust `Program.cs` for HTTP-only local IIS |

### Enable stdout logging for IIS debugging

In `publish\web.config`, set:

```xml
stdoutLogEnabled="true"
stdoutLogFile=".\logs\stdout"
```

Create the `logs` folder and restart the site. Check `publish\logs\stdout_*.log` for startup errors.

### Verify database after deployment

```sql
USE ERPHub;
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;
```

---

## Quick Reference

```powershell
# Development
cd g:\softwer\hrhub\ERP.Web
npm run build:css && dotnet build && dotnet ef database update && dotnet run --launch-profile https

# New migration
dotnet ef migrations add MigrationName
dotnet ef database update

# IIS publish
npm run build:css
dotnet publish -c Release -o g:\softwer\hrhub\publish
Restart-WebAppPool -Name "ERPHub"
```

---

*Last updated: June 2026 — ERP Hub (.NET 10 / EF Core 10 / SQL Server)*
