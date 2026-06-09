# ERP Hub — Automation Implementation Guide

This document is the single source of truth for building and extending the ERP modular monolith across automated or manual sessions.

## Prerequisites

- .NET 10 SDK
- Node.js 18+
- SQL Server Express (or LocalDB)
- PowerShell or bash terminal

## Bootstrap Commands

```bash
cd ERP.Web
npm run build:css
dotnet build
dotnet ef database update
dotnet run --launch-profile https
```

**URLs:** http://localhost:5080 | https://localhost:5081

Default login: `admin@erp.com` / `Admin@123`

## Architecture

**Pattern:** Controller → Service → Repository → AppDbContext → SQL Server

**Organogram hierarchy:** Company → Department → Section → Line (+ Designation on Employee)

## Permission Codes (Full List)

| Code | Module |
|------|--------|
| `HR.Employee.View/Create/Edit/Delete` | HR |
| `Admin.User.View/Create/Edit/Delete` | Admin |
| `Admin.Role.Manage` | Admin |
| `Company.View/Edit` | Company |
| `Organogram.Manage` | Company |
| `Shift.Manage` | Shift |
| `Attendance.Process` | Attendance |
| `Attendance.Punch.Import` | Attendance |
| `Attendance.Report.View` | Attendance |
| `Leave.Manage` | Leave |
| `Leave.Approve` | Leave |
| `Payroll.View` | Payroll |
| `Payroll.Generate` | Payroll |
| `Payroll.Advance.Manage` | Payroll |
| `Payroll.Increment.Manage` | Payroll |
| `Payroll.Bill.Config` | Payroll |

Apply with `[RequirePermission("...")]` on controllers/actions.

## Phase Checklist

### Phase 1 — Foundation (Complete)

- [x] Identity login, permissions, Employee CRUD, Dashboard, Tailwind layout

### Phases 2–5 — Core Modules (Complete)

- [x] Shift CRUD + assignments (`/Shift/Shift`)
- [x] Punch import/list/manual (`/Attendance/Punch`)
- [x] Attendance process, daily list, report, summary (`/Attendance/Attendance`)
- [x] Leave types, balances, applications, approval (`/Leave/Leave`)
- [x] Payroll generate, sheet, summary, payslip PDF (`/Payroll/Payroll`)

### Phase 6 — User & Role Admin (Complete)

- [x] `/Admin/User` — user CRUD, role assignment
- [x] `/Admin/Role` — role CRUD, permission checkbox grid
- [x] `IUserManagementService`, `IRoleManagementService`

### Phase 7 — Company & Organogram (Complete)

- [x] `/Company/Company` — company profile + addresses
- [x] `/Company/Organogram` — Department → Section → Line tree + CRUD
- [x] Designation CRUD
- [x] Entities: `Section`, `Line`, `CompanyAddress`

### Phase 8 — Employee Enhancement (Complete)

- [x] Employee Section/Line fields + cascading dropdowns (`/HR/Lookup`)
- [x] Excel export/import (EPPlus)
- [x] Employee profile PDF (QuestPDF)

### Phase 9 — Punch Collection (Complete)

- [x] Punch list, manual entry, Excel import
- [x] `PunchSource` enum (Device, Import, Manual)

### Phase 10 — Attendance Reports (Complete)

- [x] `AttendanceProcessService` — shift window, late, OT, absent
- [x] Attendance Report (detail) + Summary (aggregate)
- [x] Excel export for daily attendance

### Phase 11 — Leave Management (Complete)

- [x] `LeaveBalance` entity
- [x] Leave type CRUD, balance adjust, application, approval

### Phase 12 — Payroll & Bills (Complete)

- [x] Salary formulas (non-compliance breakdown)
- [x] Night Bill + Holiday Bill (fixed rate via `BillRateConfig`)
- [x] Advance salary + increment modules
- [x] Payslip PDF, payroll sheet Excel export

## Business Formulas

### Salary breakdown

```
Medical = 750
Food = 1250
Conveyance = 450
Basic = (Gross - 2450) / 1.5
HouseRent = Gross - Basic - 2450
OTRate = Basic / 208 * 2
OTAmount = OTRate * (OvertimeMinutes / 60)
AbsentDeduction = (Gross / payableDaysInMonth) * absentDays
```

### Night Bill (fixed rate)

```
If DailyAttendance.IsNightShift:
  PerHour: NightBill += BillRateConfig.Amount * (WorkedMinutes / 60)
  PerDay:  NightBill += BillRateConfig.Amount
```

### Holiday Bill (fixed rate)

```
If DailyAttendance.IsHoliday AND employee worked:
  HolidayBill += BillRateConfig.Amount (per day)
```

### Advance recovery

```
AdvanceDeduction = min(MonthlyDeduction, RemainingBalance)
NetPayable = Gross - AbsentDeduction + OTAmount + NightBill + HolidayBill - AdvanceDeduction
```

### Attendance processing

```
Window: ShiftStart - PunchWindowBeforeMinutes → next day cutoff
InTime = first punch in window; OutTime = last punch in window
LateMinutes = max(0, InTime - ShiftStart - GraceMinutes)
Skip if IsApproved OR IsPayrollLocked
```

## Module Routes

| Module | Base Route |
|--------|------------|
| Admin Users | `/Admin/User` |
| Admin Roles | `/Admin/Role` |
| Company | `/Company/Company` |
| Organogram | `/Company/Organogram` |
| Employees | `/HR/Employee` |
| Shifts | `/Shift/Shift` |
| Punches | `/Attendance/Punch` |
| Attendance | `/Attendance/Attendance` |
| Leave | `/Leave/Leave` |
| Payroll | `/Payroll/Payroll` |

## Migrations

| Migration | Purpose |
|-----------|---------|
| `InitialCreate` | Phase 1 schema |
| `Phase2Modules` | Section, Line, LeaveBalance, BillRateConfig, AdvanceSalary, SalaryIncrement, organogram fields |

## Test Commands

```bash
cd ERP.Web
npm run build:css
dotnet build
dotnet ef database update
dotnet run --launch-profile https
```

## Connection String

Use SQL Server Express if LocalDB is unavailable:

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ERPHub;Trusted_Connection=True;TrustServerCertificate=True"
```

## Agent Notes

1. Read this file first before extending the ERP
2. Follow Controller → Service → Repository pattern
3. Never bind entities directly in Razor forms
4. All new permissions must be added to `DbSeeder.PermissionCodes`
5. Run `dotnet build` after changes; stop running app before rebuild if file locked
