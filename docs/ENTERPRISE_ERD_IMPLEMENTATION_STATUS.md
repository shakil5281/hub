# Enterprise ERD Implementation Status

Source ERD: `C:\Users\shaki\.codex\attachments\c1e7aa08-a4aa-447a-a846-c1806bc7c343\pasted-text.txt`

## What Was Added

The larger ERD has been added as an EF Core database foundation in the app's existing `int` primary-key and `BaseEntity` style.

New migration:

```text
20260609154917_EnterpriseErdExpansion
```

The migration has been applied to the local SQL Server database `ERPHub`.

## Existing Tables Extended

These current tables were extended with fields from the pasted ERD:

| Table | Added coverage |
| --- | --- |
| `Employees` | resignation date, employee category, employment type |
| `Shifts` | shift code, punch-after window, break/minimum work/minimum OT settings, night-shift flag |
| `PunchRecords` | punch card, device linkage, device name, verification type, duplicate/processed flags, raw payload |
| `DailyAttendances` | shift/window timestamps, early-out, detailed OT buckets, night work, payable day value, present/late/holiday/weekly-off/leave flags |
| `LeaveTypes` | leave code, half-day, carry-forward, encashment flags |
| `LeaveApplications` | duration type, approval level, applied/final approval metadata |
| `LeaveBalances` | opening, earned, adjusted, closed fields |
| `PayrollDetails` | salary-structure/monthly-summary links, weekly-off bill, attendance bonus, loan deduction, detail status |

## New Tables Added

| Module | New tables |
| --- | --- |
| Employee history/profile | `EmployeeAddresses`, `EmployeeJobHistories`, `EmployeeSalaryStructures`, `EmployeeDeviceMappings`, `EmployeeWeeklyOffs` |
| Device attendance | `DeviceMasters`, `DeviceSyncLogs`, `ManualPunchRequests`, `DailyAttendancePunches` |
| Attendance approval/control | `AttendanceApprovalLogs`, `OvertimeApprovalLogs`, `AttendanceConflicts`, `MonthlyAttendanceSummaries` |
| Leave rules/audit | `LeavePolicies`, `LeaveApprovalLogs`, `LeaveBalanceTransactions`, `LeaveConflicts` |
| Bills and bonuses | `NightBillRules`, `SpecialDayBillRules`, `AttendanceBonusRules`, `EmployeeBillEntries`, `EmployeeBonusEntries` |
| Loans | `Loans`, `LoanInstallments` |
| Payroll run model | `PayrollRuns`, `PayrollRunDetails`, `Payslips` |
| System logging | `AuditLogs`, `ServiceErrorLogs` |

## Current Status

The database schema, EF entities, DbSets, relationships, indexes, decimal precision, and soft-delete filters are in place.

The application still needs service/controller/Razor workflows for the new modules. Existing screens continue to use the earlier simplified flows, while the new tables are ready for feature implementation.

## Recommended Build Order

1. Device setup and punch sync:
   `DeviceMasters`, `DeviceSyncLogs`, `EmployeeDeviceMappings`, enhanced `PunchRecords`.
2. Manual punch and attendance approvals:
   `ManualPunchRequests`, `DailyAttendancePunches`, `AttendanceApprovalLogs`, `OvertimeApprovalLogs`, `AttendanceConflicts`.
3. Monthly attendance summary:
   Generate `MonthlyAttendanceSummaries` from `DailyAttendances`.
4. Leave policy engine:
   `LeavePolicies`, `LeaveApprovalLogs`, `LeaveBalanceTransactions`, `LeaveConflicts`.
5. Payroll expansion:
   `PayrollRuns`, `PayrollRunDetails`, `Payslips`, loans, bonus rules, bill entries.
6. Audit/error logging:
   Wire `AuditLogs` and `ServiceErrorLogs` into services and middleware.

## Notes

- The pasted ERD used `uniqueidentifier` keys. The existing app uses integer keys through `BaseEntity`, so the new schema follows the existing app pattern for compatibility.
- This pass intentionally avoids removing or renaming existing properties used by current UI/services.
- The migration is additive in the forward path, except replacing simple `CompanyId` indexes on `Shifts` and `LeaveTypes` with filtered unique code indexes.
