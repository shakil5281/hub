# ERP Hub — Entity Relationship Diagram (ERD) & Data Flow

Complete database schema and application data-flow reference for the ERP Hub modular monolith.

**Database:** `ERPHub` (SQL Server)  
**ORM:** Entity Framework Core 10  
**DbContext:** `AppDbContext`  
**Pattern:** Controller → Service → Repository → `AppDbContext` → SQL Server

---

## Table of Contents

1. [High-Level Domain Overview](#1-high-level-domain-overview)
2. [Full ERD — Company & Organogram](#2-full-erd--company--organogram)
3. [Full ERD — Employee & HR](#3-full-erd--employee--hr)
4. [Full ERD — Shift & Attendance](#4-full-erd--shift--attendance)
5. [Full ERD — Leave Management](#5-full-erd--leave-management)
6. [Full ERD — Payroll & Compensation](#6-full-erd--payroll--compensation)
7. [Full ERD — Security & Identity](#7-full-erd--security--identity)
8. [Consolidated Master ERD](#8-consolidated-master-erd)
9. [Entity Reference Table](#9-entity-reference-table)
10. [Application Architecture Data Flow](#10-application-architecture-data-flow)
11. [Business Process Data Flows](#11-business-process-data-flows)
12. [Module-to-Table Mapping](#12-module-to-table-mapping)

---

## 1. High-Level Domain Overview

```mermaid
flowchart TB
    subgraph SECURITY["Security & Access"]
        USER[ApplicationUser]
        ROLE[ApplicationRole]
        PERM[Permission]
        RP[RolePermission]
    end

    subgraph COMPANY["Company & Organogram"]
        CO[Company]
        ADDR[CompanyAddress]
        DEPT[Department]
        SEC[Section]
        LINE[Line]
        DES[Designation]
    end

    subgraph HR["Human Resources"]
        EMP[Employee]
        SINC[SalaryIncrement]
        ADV[AdvanceSalary]
    end

    subgraph SHIFT["Shift Management"]
        SH[Shift]
        SA[ShiftAssignment]
        TSA[TemporaryShiftAssignment]
        HOL[Holiday]
    end

    subgraph ATT["Attendance"]
        PUNCH[PunchRecord]
        DA[DailyAttendance]
        APL[AttendanceProcessLog]
        JC[JobCard]
    end

    subgraph LEAVE["Leave"]
        LT[LeaveType]
        LA[LeaveApplication]
        LB[LeaveBalance]
    end

    subgraph PAY["Payroll"]
        PP[PayrollPeriod]
        PS[PayrollSheet]
        PD[PayrollDetail]
        BRC[BillRateConfig]
        EB[EidBonus]
    end

    subgraph UTIL["Utilities"]
        SF[SavedFilter]
    end

    CO --> DEPT --> SEC --> LINE
    SEC --> DES
    CO --> EMP
    DEPT & SEC & LINE & DES --> EMP
    EMP --> SA & TSA & PUNCH & DA & LA & LB & PD & ADV & SINC & EB & JC
    SH --> SA & TSA & BRC
    PUNCH --> DA
    DA --> PD
    PP --> PS --> PD
    USER --> ROLE
    ROLE --> RP --> PERM
    CO --> USER
```

---

## 2. Full ERD — Company & Organogram

Organogram hierarchy: **Company → Department → Section → Line**  
Designations belong to a **Section** and employees are assigned to all four levels.

```mermaid
erDiagram
    Company {
        int Id PK
        string Name
        string Code
        string Address
        string Phone
        string Email
        string LogoPath
        string RegistrationNo
        string TaxId
        datetime CreatedAt
        string CreatedBy
        datetime UpdatedAt
        string UpdatedBy
        bool IsDeleted
        int Status
    }

    CompanyAddress {
        int Id PK
        int CompanyId FK
        string Label
        string AddressLine1
        string AddressLine2
        string City
        string State
        string PostalCode
        bool IsPrimary
        datetime CreatedAt
        bool IsDeleted
        int Status
    }

    Department {
        int Id PK
        int CompanyId FK
        string Name
        string Code
        datetime CreatedAt
        bool IsDeleted
        int Status
    }

    Section {
        int Id PK
        int CompanyId FK
        int DepartmentId FK
        string Name
        string Code
        datetime CreatedAt
        bool IsDeleted
        int Status
    }

    Line {
        int Id PK
        int CompanyId FK
        int SectionId FK
        string Name
        string Code
        datetime CreatedAt
        bool IsDeleted
        int Status
    }

    Designation {
        int Id PK
        int CompanyId FK
        int SectionId FK
        string Title
        string Code
        datetime CreatedAt
        bool IsDeleted
        int Status
    }

    Company ||--o{ CompanyAddress : "has addresses"
    Company ||--o{ Department : "has departments"
    Department ||--o{ Section : "has sections"
    Section ||--o{ Line : "has lines"
    Section ||--o{ Designation : "has designations"
```

**Unique indexes:**
- `Department`: `(CompanyId, Code)`
- `Section`: `(DepartmentId, Code)`
- `Line`: `(SectionId, Code)`
- `Designation`: `(CompanyId, Code)`

---

## 3. Full ERD — Employee & HR

Central entity connecting organogram, attendance, leave, and payroll.

```mermaid
erDiagram
    Employee {
        int Id PK
        int CompanyId FK
        string EmployeeCode UK
        string PunchNumber UK
        string FullName
        datetime DateOfBirth
        string Gender
        string BloodGroup
        string NationalId
        string Phone
        string Email
        string MaritalStatus
        int DepartmentId FK
        int SectionId FK
        int LineId FK
        int DesignationId FK
        int AddressId FK
        string PresentAddress
        string PermanentAddress
        datetime JoiningDate
        datetime ConfirmationDate
        decimal GrossSalary
        decimal BasicSalary
        decimal HouseRentAllowance
        decimal MedicalAllowance
        decimal TransportAllowance
        decimal OtherAllowance
        string BankName
        string BankAccountNo
        string BankBranch
        string TaxId
        string FatherName
        string MotherName
        string SpouseName
        string EmergencyContactName
        string EmergencyContactPhone
        string EmergencyContactRelation
        string SignatureData
        datetime CreatedAt
        bool IsDeleted
        int Status
    }

    Company {
        int Id PK
        string Name
        string Code
    }

    Department {
        int Id PK
        string Name
        string Code
    }

    Section {
        int Id PK
        string Name
        string Code
    }

    Line {
        int Id PK
        string Name
        string Code
    }

    Designation {
        int Id PK
        string Title
        string Code
    }

    CompanyAddress {
        int Id PK
        string Label
        string AddressLine1
        string City
    }

    SalaryIncrement {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        decimal PreviousGross
        decimal NewGross
        datetime EffectiveDate
        string Reason
        datetime CreatedAt
        bool IsDeleted
    }

    AdvanceSalary {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        decimal Amount
        datetime RequestDate
        datetime ApprovedDate
        decimal MonthlyDeduction
        decimal RemainingBalance
        int AdvanceStatus
        string Reason
        datetime CreatedAt
        bool IsDeleted
    }

    Company ||--o{ Employee : "employs"
    Department ||--o{ Employee : "assigned to"
    Section ||--o{ Employee : "assigned to"
    Line ||--o{ Employee : "assigned to"
    Designation ||--o{ Employee : "holds"
    CompanyAddress |o--o| Employee : "optional address"
    Employee ||--o{ SalaryIncrement : "receives"
    Employee ||--o{ AdvanceSalary : "requests"
```

**Unique indexes:**
- `Employee`: `(CompanyId, EmployeeCode)`, `(CompanyId, PunchNumber)`

---

## 4. Full ERD — Shift & Attendance

Raw punch data is processed into daily attendance records used by payroll.

```mermaid
erDiagram
    Employee {
        int Id PK
        string EmployeeCode
        string PunchNumber
        string FullName
    }

    Shift {
        int Id PK
        int CompanyId FK
        string Name
        time StartTime
        time EndTime
        int GraceMinutes
        int PunchWindowBeforeMinutes
        int MaxOvertimeMinutes
        datetime CreatedAt
        bool IsDeleted
    }

    ShiftAssignment {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        int ShiftId FK
        datetime EffectiveFrom
        datetime EffectiveTo
        datetime CreatedAt
        bool IsDeleted
    }

    TemporaryShiftAssignment {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        int ShiftId FK
        datetime AssignmentDate
        string Reason
        datetime CreatedAt
        bool IsDeleted
    }

    Holiday {
        int Id PK
        int CompanyId FK
        string Name
        datetime HolidayDate
        int HolidayType
        string Description
        datetime CreatedAt
        bool IsDeleted
    }

    PunchRecord {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        datetime PunchTime
        int PunchType
        int Source
        string DeviceId
        datetime CreatedAt
        bool IsDeleted
    }

    DailyAttendance {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        datetime AttendanceDate
        datetime InTime
        datetime OutTime
        int LateMinutes
        int OvertimeMinutes
        int WorkedMinutes
        bool IsAbsent
        bool IsHoliday
        bool IsNightShift
        bool IsApproved
        bool IsPayrollLocked
        datetime CreatedAt
        bool IsDeleted
    }

    AttendanceProcessLog {
        int Id PK
        int CompanyId FK
        datetime FromDate
        datetime ToDate
        int ProcessedRows
        int SkippedRows
        datetime CreatedAt
        bool IsDeleted
    }

    JobCard {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        int LineId FK
        datetime WorkDate
        string TaskDescription
        decimal TargetQty
        decimal AchievedQty
        string Remarks
        datetime CreatedAt
        bool IsDeleted
    }

    Employee ||--o{ ShiftAssignment : "assigned"
    Shift ||--o{ ShiftAssignment : "used in"
    Employee ||--o{ TemporaryShiftAssignment : "temp assigned"
    Shift ||--o{ TemporaryShiftAssignment : "used in"
    Employee ||--o{ PunchRecord : "punches"
    Employee ||--o{ DailyAttendance : "has daily"
    Employee ||--o{ JobCard : "works on"
    Line ||--o{ JobCard : "production line"
```

**Unique indexes:**
- `DailyAttendance`: `(CompanyId, EmployeeId, AttendanceDate)`
- `TemporaryShiftAssignment`: `(EmployeeId, AssignmentDate)`
- `Holiday`: `(CompanyId, HolidayDate)`
- `JobCard`: `(CompanyId, WorkDate)`

**Enums:**
- `PunchType`: In / Out
- `PunchSource`: Device, Import, Manual
- `HolidayType`: Public, Company, Optional

---

## 5. Full ERD — Leave Management

```mermaid
erDiagram
    Employee {
        int Id PK
        string FullName
        string EmployeeCode
    }

    LeaveType {
        int Id PK
        int CompanyId FK
        string Name
        int MaxDaysPerYear
        bool IsPaid
        datetime CreatedAt
        bool IsDeleted
    }

    LeaveApplication {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        int LeaveTypeId FK
        datetime FromDate
        datetime ToDate
        int TotalDays
        int ApprovalStatus
        string Reason
        datetime CreatedAt
        bool IsDeleted
    }

    LeaveBalance {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        int LeaveTypeId FK
        int Year
        int AllocatedDays
        int UsedDays
        int RemainingDays
        datetime CreatedAt
        bool IsDeleted
    }

    Employee ||--o{ LeaveApplication : "applies"
    LeaveType ||--o{ LeaveApplication : "type of"
    Employee ||--o{ LeaveBalance : "has balance"
    LeaveType ||--o{ LeaveBalance : "balance for"
```

**Unique index:**
- `LeaveBalance`: `(CompanyId, EmployeeId, LeaveTypeId, Year)`

**Enum:**
- `LeaveApprovalStatus`: Pending, Approved, Rejected

---

## 6. Full ERD — Payroll & Compensation

```mermaid
erDiagram
    Employee {
        int Id PK
        string FullName
        decimal GrossSalary
        decimal BasicSalary
    }

    PayrollPeriod {
        int Id PK
        int CompanyId FK
        string Name
        datetime StartDate
        datetime EndDate
        int PayrollStatus
        datetime CreatedAt
        bool IsDeleted
    }

    PayrollSheet {
        int Id PK
        int CompanyId FK
        int PayrollPeriodId FK
        int PayrollStatus
        decimal TotalNetPayable
        datetime CreatedAt
        bool IsDeleted
    }

    PayrollDetail {
        int Id PK
        int CompanyId FK
        int PayrollSheetId FK
        int EmployeeId FK
        decimal GrossSalary
        decimal BasicSalary
        decimal HouseRent
        decimal MedicalAllowance
        decimal FoodAllowance
        decimal ConveyanceAllowance
        int AttendancePayableDays
        decimal AbsentDeduction
        decimal OvertimeAmount
        decimal NightBillAmount
        decimal HolidayBillAmount
        decimal AdvanceDeduction
        decimal IncrementAdjustment
        decimal NetPayableSalary
        datetime CreatedAt
        bool IsDeleted
    }

    BillRateConfig {
        int Id PK
        int CompanyId FK
        int BillType
        int RateType
        decimal Amount
        int ShiftId FK
        datetime CreatedAt
        bool IsDeleted
    }

    AdvanceSalary {
        int Id PK
        int EmployeeId FK
        decimal Amount
        decimal MonthlyDeduction
        decimal RemainingBalance
        int AdvanceStatus
    }

    SalaryIncrement {
        int Id PK
        int EmployeeId FK
        decimal PreviousGross
        decimal NewGross
        datetime EffectiveDate
    }

    EidBonus {
        int Id PK
        int CompanyId FK
        int EmployeeId FK
        int BonusType
        int Year
        decimal BonusAmount
        int BonusStatus
        string Remarks
        datetime CreatedAt
        bool IsDeleted
    }

    Shift {
        int Id PK
        string Name
    }

    PayrollPeriod ||--o{ PayrollSheet : "generates"
    PayrollSheet ||--o{ PayrollDetail : "contains"
    Employee ||--o{ PayrollDetail : "paid in"
    Employee ||--o{ AdvanceSalary : "owes"
    Employee ||--o{ SalaryIncrement : "incremented"
    Employee ||--o{ EidBonus : "receives bonus"
    Shift |o--o{ BillRateConfig : "optional shift rate"
```

**Enums:**
- `PayrollStatus`: Draft, Generated, Locked
- `BillType`: Night, Holiday
- `BillRateType`: PerHour, PerDay
- `AdvanceSalaryStatus`: Pending, Approved, Rejected, Completed
- `BonusType`: EidUlFitr, EidUlAdha
- `BonusStatus`: Draft, Approved, Paid

**Salary formula (PayrollService):**
```
Medical = 750 | Food = 1250 | Conveyance = 450
Basic = (Gross - 2450) / 1.5
HouseRent = Gross - Basic - 2450
OTRate = Basic / 208 * 2
NetPayable = Gross - AbsentDeduction + OT + NightBill + HolidayBill - AdvanceDeduction
```

---

## 7. Full ERD — Security & Identity

ASP.NET Core Identity tables plus custom permission-based authorization.

```mermaid
erDiagram
    ApplicationUser {
        string Id PK
        int CompanyId FK
        string FullName
        string UserName UK
        string Email UK
        string PasswordHash
        string PhoneNumber
        bool EmailConfirmed
        bool LockoutEnabled
    }

    ApplicationRole {
        string Id PK
        string Name UK
        string Description
        string NormalizedName
    }

    IdentityUserRole {
        string UserId FK
        string RoleId FK
    }

    IdentityUserClaim {
        int Id PK
        string UserId FK
        string ClaimType
        string ClaimValue
    }

    IdentityRoleClaim {
        int Id PK
        string RoleId FK
        string ClaimType
        string ClaimValue
    }

    Permission {
        int Id PK
        string Code UK
        string Name
        string Module
    }

    RolePermission {
        int Id PK
        string RoleId FK
        int PermissionId FK
    }

    SavedFilter {
        int Id PK
        int CompanyId FK
        string UserId FK
        string Module
        string Name
        string FilterJson
        bool IsDefault
        datetime CreatedAt
        bool IsDeleted
    }

    Company {
        int Id PK
        string Name
    }

    Company ||--o{ ApplicationUser : "belongs to"
    ApplicationUser ||--o{ IdentityUserRole : "has roles"
    ApplicationRole ||--o{ IdentityUserRole : "assigned to users"
    ApplicationRole ||--o{ RolePermission : "grants"
    Permission ||--o{ RolePermission : "granted via"
    ApplicationUser ||--o{ SavedFilter : "saves filters"
```

**Identity tables (auto-managed):**
| Table | Purpose |
|-------|---------|
| `AspNetUsers` | `ApplicationUser` — login accounts |
| `AspNetRoles` | `ApplicationRole` — role definitions |
| `AspNetUserRoles` | User ↔ Role many-to-many |
| `AspNetUserClaims` | Per-user claims |
| `AspNetRoleClaims` | Per-role claims |
| `AspNetUserLogins` | External login providers |
| `AspNetUserTokens` | Password reset / 2FA tokens |

**Unique indexes:**
- `Permission.Code`
- `RolePermission`: `(RoleId, PermissionId)`
- `SavedFilter`: `(CompanyId, UserId, Module, Name)`

---

## 8. Consolidated Master ERD

Single diagram showing all primary foreign-key relationships across the entire `ERPHub` database.

```mermaid
erDiagram
    Company ||--o{ CompanyAddress : has
    Company ||--o{ Department : has
    Company ||--o{ Employee : employs
    Company ||--o{ Shift : defines
    Company ||--o{ LeaveType : defines
    Company ||--o{ PayrollPeriod : defines
    Company ||--o{ Holiday : defines
    Company ||--o{ BillRateConfig : configures
    Company ||--o{ ApplicationUser : scopes

    Department ||--o{ Section : contains
    Section ||--o{ Line : contains
    Section ||--o{ Designation : contains

    Department ||--o{ Employee : dept
    Section ||--o{ Employee : section
    Line ||--o{ Employee : line
    Designation ||--o{ Employee : title
    CompanyAddress |o--o| Employee : address

    Employee ||--o{ ShiftAssignment : shift
    Shift ||--o{ ShiftAssignment : assigned
    Employee ||--o{ TemporaryShiftAssignment : temp_shift
    Shift ||--o{ TemporaryShiftAssignment : temp_shift

    Employee ||--o{ PunchRecord : punches
    Employee ||--o{ DailyAttendance : attendance
    Employee ||--o{ LeaveApplication : leave_app
    LeaveType ||--o{ LeaveApplication : type
    Employee ||--o{ LeaveBalance : leave_bal
    LeaveType ||--o{ LeaveBalance : type

    PayrollPeriod ||--o{ PayrollSheet : sheet
    PayrollSheet ||--o{ PayrollDetail : detail
    Employee ||--o{ PayrollDetail : paid
    Employee ||--o{ AdvanceSalary : advance
    Employee ||--o{ SalaryIncrement : increment
    Employee ||--o{ EidBonus : bonus
    Employee ||--o{ JobCard : jobcard
    Line ||--o{ JobCard : line
    Shift |o--o{ BillRateConfig : rate

    ApplicationRole ||--o{ RolePermission : grants
    Permission ||--o{ RolePermission : code
    ApplicationUser ||--o{ SavedFilter : filters
```

---

## 9. Entity Reference Table

All 30 domain entities + 7 Identity tables.

| # | Entity | Table Name | PK | Key FKs | Module |
|---|--------|------------|----|---------|--------|
| 1 | `Company` | Companies | Id | — | Company |
| 2 | `CompanyAddress` | CompanyAddresses | Id | CompanyId | Company |
| 3 | `Department` | Departments | Id | CompanyId | Organogram |
| 4 | `Section` | Sections | Id | CompanyId, DepartmentId | Organogram |
| 5 | `Line` | Lines | Id | CompanyId, SectionId | Organogram |
| 6 | `Designation` | Designations | Id | CompanyId, SectionId | Organogram |
| 7 | `Employee` | Employees | Id | CompanyId, Dept, Section, Line, Designation, AddressId | HR |
| 8 | `Shift` | Shifts | Id | CompanyId | Shift |
| 9 | `ShiftAssignment` | ShiftAssignments | Id | EmployeeId, ShiftId | Shift |
| 10 | `TemporaryShiftAssignment` | TemporaryShiftAssignments | Id | EmployeeId, ShiftId | Shift |
| 11 | `Holiday` | Holidays | Id | CompanyId | Leave |
| 12 | `PunchRecord` | PunchRecords | Id | EmployeeId | Attendance |
| 13 | `DailyAttendance` | DailyAttendances | Id | EmployeeId | Attendance |
| 14 | `AttendanceProcessLog` | AttendanceProcessLogs | Id | CompanyId | Attendance |
| 15 | `JobCard` | JobCards | Id | EmployeeId, LineId | Attendance |
| 16 | `LeaveType` | LeaveTypes | Id | CompanyId | Leave |
| 17 | `LeaveApplication` | LeaveApplications | Id | EmployeeId, LeaveTypeId | Leave |
| 18 | `LeaveBalance` | LeaveBalances | Id | EmployeeId, LeaveTypeId | Leave |
| 19 | `PayrollPeriod` | PayrollPeriods | Id | CompanyId | Payroll |
| 20 | `PayrollSheet` | PayrollSheets | Id | PayrollPeriodId | Payroll |
| 21 | `PayrollDetail` | PayrollDetails | Id | PayrollSheetId, EmployeeId | Payroll |
| 22 | `BillRateConfig` | BillRateConfigs | Id | CompanyId, ShiftId? | Payroll |
| 23 | `AdvanceSalary` | AdvanceSalaries | Id | EmployeeId | Payroll |
| 24 | `SalaryIncrement` | SalaryIncrements | Id | EmployeeId | Payroll |
| 25 | `EidBonus` | EidBonuses | Id | EmployeeId | Payroll |
| 26 | `SavedFilter` | SavedFilters | Id | CompanyId, UserId | System |
| 27 | `Permission` | Permissions | Id | — | Security |
| 28 | `RolePermission` | RolePermissions | Id | RoleId, PermissionId | Security |
| 29 | `ApplicationUser` | AspNetUsers | Id (string) | CompanyId | Security |
| 30 | `ApplicationRole` | AspNetRoles | Id (string) | — | Security |

**BaseEntity fields** (inherited by most entities):  
`Id`, `CompanyId`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `IsDeleted`, `Status`

**Soft-delete:** Global query filters on `IsDeleted = false` for all business entities.

---

## 10. Application Architecture Data Flow

### 10.1 — Request Pipeline (All Modules)

```mermaid
flowchart LR
    subgraph CLIENT["Client Layer"]
        BROWSER[Browser / User]
        RAZOR[Razor Views .cshtml]
        JS[JavaScript / erp.js]
        CSS[Tailwind CSS site.css]
    end

    subgraph WEB["ASP.NET Core MVC"]
        CTRL[Area Controllers]
        AUTH[RequirePermission Filter]
        IDENTITY[ASP.NET Identity]
        CLAIMS[Permission Claims Factory]
    end

    subgraph APP["Application Layer"]
        SVC[Services]
        DTO[DTOs / ViewModels]
        EXPORT[Excel / PDF Export]
    end

    subgraph DATA["Data Layer"]
        REPO[Repositories]
        UOW[Unit of Work]
        CTX[AppDbContext]
        SEED[DbSeeder]
    end

    subgraph DB["SQL Server"]
        SQL[(ERPHub Database)]
        MIG[__EFMigrationsHistory]
    end

    BROWSER --> RAZOR
    BROWSER --> JS
    RAZOR --> CSS
    BROWSER -->|HTTP Request| CTRL
    CTRL --> AUTH
    AUTH --> IDENTITY
    IDENTITY --> CLAIMS
    CTRL --> SVC
    SVC --> DTO
    SVC --> EXPORT
    SVC --> REPO
    SVC --> CTX
    REPO --> UOW
    REPO --> CTX
    CTX --> SQL
    SEED -->|MigrateAsync on startup| MIG
    SEED --> SQL
    CTRL -->|ViewResult / FileResult| RAZOR
```

### 10.2 — Layer Responsibilities

| Layer | Components | Responsibility |
|-------|------------|----------------|
| **Presentation** | Controllers, Razor Views, `erp.js` | HTTP handling, UI rendering, form binding |
| **Authorization** | `[RequirePermission]`, `PermissionAuthorizationHandler` | Enforce permission codes per action |
| **Application** | `*Service` classes | Business logic, validation, calculations |
| **Data Access** | `*Repository`, `IRepository<T>`, `UnitOfWork` | CRUD, queries, transactions |
| **Persistence** | `AppDbContext`, EF Core Migrations | ORM mapping, schema versioning |
| **Database** | SQL Server `ERPHub` | Persistent storage |

### 10.3 — Service-to-Entity Map

```mermaid
flowchart TB
    subgraph CTRL["Controllers (Areas)"]
        C_HR[HR/EmployeeController]
        C_ATT[Attendance/AttendanceController]
        C_PUNCH[Attendance/PunchController]
        C_LEAVE[Leave/LeaveController]
        C_PAY[Payroll/PayrollController]
        C_SHIFT[Shift/ShiftController]
        C_COMP[Company/CompanyController]
        C_ADMIN[Admin/UserController]
    end

    subgraph SVC["Services"]
        S_EMP[EmployeeService]
        S_ATT[AttendanceProcessService]
        S_PUNCH[PunchRecordService / PunchImportService]
        S_LEAVE[LeaveService]
        S_PAY[PayrollService]
        S_SHIFT[ShiftService]
        S_COMP[CompanyService / OrganogramService]
        S_USER[UserManagementService / RoleManagementService]
    end

    subgraph DB_ENT["Primary Tables"]
        T_EMP[Employees]
        T_DA[DailyAttendances]
        T_PUNCH[PunchRecords]
        T_LA[LeaveApplications]
        T_PD[PayrollDetails]
        T_SA[ShiftAssignments]
        T_CO[Companies / Departments / Sections / Lines]
        T_USER[AspNetUsers / Permissions]
    end

    C_HR --> S_EMP --> T_EMP
    C_ATT --> S_ATT --> T_DA
    C_PUNCH --> S_PUNCH --> T_PUNCH
    C_LEAVE --> S_LEAVE --> T_LA
    C_PAY --> S_PAY --> T_PD
    C_SHIFT --> S_SHIFT --> T_SA
    C_COMP --> S_COMP --> T_CO
    C_ADMIN --> S_USER --> T_USER
```

---

## 11. Business Process Data Flows

### 11.1 — Authentication & Authorization Flow

```mermaid
sequenceDiagram
    actor User
    participant Login as AccountController
    participant Identity as ASP.NET Identity
    participant Claims as ApplicationUserClaimsPrincipalFactory
    participant DB as AspNetUsers / RolePermissions
    participant Ctrl as Protected Controller
    participant Handler as PermissionAuthorizationHandler

    User->>Login: POST /Account/Login
    Login->>Identity: SignInAsync(email, password)
    Identity->>DB: Validate user + roles
    Identity->>Claims: Build claims principal
    Claims->>DB: Load RolePermissions → Permission codes
    Claims-->>User: Auth cookie with permission claims
    User->>Ctrl: Request protected action
    Ctrl->>Handler: Check [RequirePermission("HR.Employee.View")]
    Handler->>Handler: Match claim to policy
    Handler-->>Ctrl: Authorized / Forbidden
    Ctrl-->>User: View or 403 Access Denied
```

### 11.2 — Punch → Attendance Processing Flow

```mermaid
flowchart TD
    START([Start Attendance Process]) --> INPUT[User selects FromDate / ToDate]
    INPUT --> LOAD_EMP[Load Employees by CompanyId]
    LOAD_EMP --> LOAD_SHIFT[Load ShiftAssignments + Shifts]
    LOAD_SHIFT --> LOAD_PUNCH[Load PunchRecords in date window]
    LOAD_PUNCH --> LOAD_EXIST[Load existing DailyAttendances]

    LOAD_EXIST --> LOOP{For each Employee × Date}
    LOOP --> CHECK_LOCK{IsApproved or IsPayrollLocked?}
    CHECK_LOCK -->|Yes| SKIP[Skip row]
    CHECK_LOCK -->|No| GET_SHIFT[Resolve effective Shift for date]
    GET_SHIFT --> WINDOW[Calculate punch window:<br/>ShiftStart - PunchWindowBefore → next day cutoff]
    WINDOW --> FILTER[Filter punches in window]
    FILTER --> CALC[Calculate InTime, OutTime,<br/>LateMinutes, OvertimeMinutes, WorkedMinutes]
    CALC --> HOLIDAY[Check Holiday table → IsHoliday]
    HOLIDAY --> ABSENT[Determine IsAbsent]
    ABSENT --> UPSERT[Insert or Update DailyAttendance]
    UPSERT --> LOOP
    SKIP --> LOOP
    LOOP -->|Done| LOG[Write AttendanceProcessLog]
    LOG --> END([Return Processed / Skipped counts])
```

**Data inputs:** `Employees`, `ShiftAssignments`, `Shifts`, `PunchRecords`, `Holidays`  
**Data output:** `DailyAttendances`, `AttendanceProcessLogs`

### 11.3 — Payroll Generation Flow

```mermaid
flowchart TD
    START([Generate Payroll]) --> PERIOD[Select PayrollPeriod]
    PERIOD --> CHECK[Check existing PayrollSheet not Locked]
    CHECK --> CREATE[Create or reset PayrollSheet]
    CREATE --> EMP[Load all Employees]
    EMP --> ATT[Load DailyAttendances for period range]
    ATT --> ADV[Load active AdvanceSalaries]
    ADV --> INC[Load SalaryIncrements in period]
    INC --> BILL[Load BillRateConfigs Night/Holiday]

    BILL --> LOOP{For each Employee}
    LOOP --> SALARY[Split Gross into Basic, HRA, Medical, Food, Conveyance]
    SALARY --> DAYS[Count payable days from DailyAttendance]
    DAYS --> ABSENT[Calculate AbsentDeduction]
    ABSENT --> OT[Calculate OvertimeAmount from OT minutes]
    OT --> NIGHT[Calculate NightBillAmount]
    NIGHT --> HOL[Calculate HolidayBillAmount]
    HOL --> ADV_DED[Calculate AdvanceDeduction]
    ADV_DED --> NET[NetPayable = Gross - deductions + additions]
    NET --> DETAIL[Create PayrollDetail record]
    DETAIL --> LOCK[Set DailyAttendance.IsPayrollLocked = true]
    LOCK --> LOOP
    LOOP -->|Done| TOTAL[Sum TotalNetPayable on PayrollSheet]
    TOTAL --> END([Payroll Generated])
```

**Data inputs:** `PayrollPeriods`, `Employees`, `DailyAttendances`, `AdvanceSalaries`, `SalaryIncrements`, `BillRateConfigs`  
**Data output:** `PayrollSheets`, `PayrollDetails`, locked `DailyAttendances`

### 11.4 — Leave Application & Approval Flow

```mermaid
flowchart LR
    A[Employee submits LeaveApplication] --> B{ApprovalStatus = Pending}
    B --> C[Manager reviews in Leave module]
    C -->|Approve| D[Update ApprovalStatus = Approved]
    C -->|Reject| E[Update ApprovalStatus = Rejected]
    D --> F[Deduct days from LeaveBalance.RemainingDays]
    F --> G[Increment LeaveBalance.UsedDays]
    D --> H[Mark affected DailyAttendance as approved/absent]
```

### 11.5 — Employee Onboarding Data Flow

```mermaid
flowchart LR
    A[Create Organogram:<br/>Dept → Section → Line] --> B[Create Designation]
    B --> C[Create Employee record]
    C --> D[Assign Shift via ShiftAssignment]
    D --> E[Allocate LeaveBalance per LeaveType]
    E --> F[Create ApplicationUser account optional]
    F --> G[Assign Role + Permissions]
```

### 11.6 — End-to-End Monthly HR Cycle

```mermaid
flowchart TB
    subgraph WEEKLY["Daily / Weekly"]
        P1[Import / Manual PunchRecords]
        P2[AttendanceProcessService]
        P3[DailyAttendances updated]
        P1 --> P2 --> P3
    end

    subgraph LEAVE_FLOW["As Needed"]
        L1[LeaveApplication submitted]
        L2[Manager approves]
        L3[LeaveBalance updated]
        L1 --> L2 --> L3
    end

    subgraph MONTH_END["Month End"]
        M1[Review Attendance Reports]
        M2[Generate PayrollSheet]
        M3[PayrollDetails + Payslip PDF]
        M4[Export Excel payroll sheet]
        M1 --> M2 --> M3 --> M4
    end

    P3 --> M1
    L3 --> M1
    M2 -.->|reads| P3
    M2 -.->|reads| L3
```

### 11.7 — Export Data Flow

```mermaid
flowchart LR
    CTRL[Controller action] --> SVC[Service queries data]
    SVC --> EXCEL[ExcelExportService<br/>EPPlus]
    SVC --> PDF[PdfExportService<br/>QuestPDF]
    EXCEL --> FILE1[.xlsx FileResult]
    PDF --> FILE2[.pdf FileResult]
    FILE1 & FILE2 --> BROWSER[Browser download]
```

**Export targets:** Employee list, punch records, attendance reports, payroll sheets, employee profile PDF, payslip PDF.

---

## 12. Module-to-Table Mapping

| MVC Area / Route | Controller | Service | Primary Tables |
|------------------|------------|---------|----------------|
| `/HR/Employee` | EmployeeController | EmployeeService | Employees, Departments, Sections, Lines, Designations |
| `/Company/Company` | CompanyController | CompanyService, AddressService | Companies, CompanyAddresses |
| `/Company/Organogram` | OrganogramController | OrganogramService | Departments, Sections, Lines, Designations |
| `/Shift/Shift` | ShiftController | ShiftService | Shifts, ShiftAssignments, TemporaryShiftAssignments |
| `/Attendance/Punch` | PunchController | PunchRecordService, PunchImportService | PunchRecords |
| `/Attendance/Attendance` | AttendanceController | AttendanceProcessService | DailyAttendances, AttendanceProcessLogs, Holidays, JobCards |
| `/Leave/Leave` | LeaveController | LeaveService | LeaveTypes, LeaveApplications, LeaveBalances, Holidays |
| `/Payroll/Payroll` | PayrollController | PayrollService | PayrollPeriods, PayrollSheets, PayrollDetails, BillRateConfigs, AdvanceSalaries, SalaryIncrements, EidBonuses |
| `/Reports/MonthlyReport` | MonthlyReportController | MonthlyReportService | DailyAttendances, PayrollDetails, Employees |
| `/Admin/User` | UserController | UserManagementService | AspNetUsers, AspNetUserRoles |
| `/Admin/Role` | RoleController | RoleManagementService | AspNetRoles, RolePermissions, Permissions |
| `/Account/Login` | AccountController | ASP.NET Identity | AspNetUsers |

---

## Viewing the Diagrams

These diagrams use **Mermaid** syntax. They render automatically in:

- GitHub / GitLab markdown viewers
- VS Code with a Mermaid preview extension
- Cursor markdown preview
- [Mermaid Live Editor](https://mermaid.live)

---

*Generated from ERP Hub source: `ERP.Web/Core/Entities`, `Infrastructure/Data/Configurations`, and `Infrastructure/Services` — June 2026*
