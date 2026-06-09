using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Web.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseErdExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shifts_CompanyId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_LeaveTypes_CompanyId",
                table: "LeaveTypes");

            migrationBuilder.AddColumn<int>(
                name: "BreakMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsNightShift",
                table: "Shifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MinimumOtMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumWorkMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PunchAfterMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShiftCode",
                table: "Shifts",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceMasterId",
                table: "PunchRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                table: "PunchRecords",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeDeviceMappingId",
                table: "PunchRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDuplicate",
                table: "PunchRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "PunchRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PunchCardNo",
                table: "PunchRecords",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawPayload",
                table: "PunchRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifyType",
                table: "PunchRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AttendanceBonusAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DetailStatus",
                table: "PayrollDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanDeduction",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyAttendanceSummaryId",
                table: "PayrollDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalaryStructureId",
                table: "PayrollDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WeeklyOffBillAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsCarryForwardAllowed",
                table: "LeaveTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEncashmentAllowed",
                table: "LeaveTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHalfDayAllowed",
                table: "LeaveTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LeaveCode",
                table: "LeaveTypes",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AdjustedDays",
                table: "LeaveBalances",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EarnedDays",
                table: "LeaveBalances",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "LeaveBalances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningBalance",
                table: "LeaveBalances",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "AppliedAt",
                table: "LeaveApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppliedBy",
                table: "LeaveApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentApprovalLevel",
                table: "LeaveApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalApprovedAt",
                table: "LeaveApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalApprovedBy",
                table: "LeaveApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeaveDurationType",
                table: "LeaveApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCategory",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmploymentType",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResignDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedOvertimeMinutes",
                table: "DailyAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AttendanceStatus",
                table: "DailyAttendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMinutes",
                table: "DailyAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EarlyOutMinutes",
                table: "DailyAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsEarlyOut",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHolidayPresent",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLate",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeave",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNightDuty",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidLeave",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPresent",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWeeklyOff",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWeeklyOffPresent",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NightWorkMinutes",
                table: "DailyAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PayableDayValue",
                table: "DailyAttendances",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PayableOvertimeMinutes",
                table: "DailyAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RawOvertimeMinutes",
                table: "DailyAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RuleOvertimeMinutes",
                table: "DailyAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShiftEndDateTime",
                table: "DailyAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShiftId",
                table: "DailyAttendances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShiftStartDateTime",
                table: "DailyAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WindowEndDateTime",
                table: "DailyAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WindowStartDateTime",
                table: "DailyAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendanceApprovalLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyAttendanceId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceApprovalLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceApprovalLogs_DailyAttendances_DailyAttendanceId",
                        column: x => x.DailyAttendanceId,
                        principalTable: "DailyAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceBonusRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    RuleName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BonusAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxAbsentAllowed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxUnpaidLeaveAllowed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxLateAllowed = table.Column<int>(type: "int", nullable: false),
                    MaxEarlyOutAllowed = table.Column<int>(type: "int", nullable: false),
                    MaxMissingPunchAllowed = table.Column<int>(type: "int", nullable: false),
                    MinimumPresentDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumPayableDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceBonusRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceBonusRules_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceBonusRules_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceConflicts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConflictType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConflictStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceConflicts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyAttendancePunches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyAttendanceId = table.Column<int>(type: "int", nullable: false),
                    PunchRecordId = table.Column<int>(type: "int", nullable: false),
                    PunchUsageType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyAttendancePunches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyAttendancePunches_DailyAttendances_DailyAttendanceId",
                        column: x => x.DailyAttendanceId,
                        principalTable: "DailyAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DailyAttendancePunches_PunchRecords_PunchRecordId",
                        column: x => x.PunchRecordId,
                        principalTable: "PunchRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DeviceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceMasters_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Division = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Upazila = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAddresses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBillEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DailyAttendanceId = table.Column<int>(type: "int", nullable: true),
                    BillType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BillYear = table.Column<int>(type: "int", nullable: false),
                    BillMonth = table.Column<int>(type: "int", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceReferenceId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EntryStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPayrollLocked = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBillEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeBillEntries_DailyAttendances_DailyAttendanceId",
                        column: x => x.DailyAttendanceId,
                        principalTable: "DailyAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeBillEntries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDeviceMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PunchCardNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DeviceUserId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDeviceMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDeviceMappings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeJobHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    LineId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeJobHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeJobHistories_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeJobHistories_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeJobHistories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeJobHistories_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeJobHistories_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HouseRent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MedicalAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FoodAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ConveyanceAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaryStructures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryStructures_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeWeeklyOffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    OffDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OffType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeWeeklyOffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeWeeklyOffs_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveApprovalLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApprovalLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveApprovalLogs_LeaveApplications_LeaveApplicationId",
                        column: x => x.LeaveApplicationId,
                        principalTable: "LeaveApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveBalanceTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    LeaveApplicationId = table.Column<int>(type: "int", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Days = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalanceTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveBalanceTransactions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveBalanceTransactions_LeaveApplications_LeaveApplicationId",
                        column: x => x.LeaveApplicationId,
                        principalTable: "LeaveApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveBalanceTransactions_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveConflicts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveApplicationId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ConflictDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConflictType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConflictStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolutionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveConflicts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveConflicts_LeaveApplications_LeaveApplicationId",
                        column: x => x.LeaveApplicationId,
                        principalTable: "LeaveApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeavePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    PolicyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearlyQuota = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyAccrual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxCarryForwardDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxEncashmentDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExcludeHolidayFromLeave = table.Column<bool>(type: "bit", nullable: false),
                    ExcludeWeeklyOffFromLeave = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePolicies_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LoanStatus = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ManualPunchRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RequestedPunchTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestStatus = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedPunchRecordId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualPunchRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualPunchRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPunchRequests_PunchRecords_CreatedPunchRecordId",
                        column: x => x.CreatedPunchRecordId,
                        principalTable: "PunchRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyAttendanceSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SalaryYear = table.Column<int>(type: "int", nullable: false),
                    SalaryMonth = table.Column<int>(type: "int", nullable: false),
                    CalendarDays = table.Column<int>(type: "int", nullable: false),
                    PayableDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PresentDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AbsentDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidLeaveDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnpaidLeaveDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HolidayDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HolidayPresentDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WeeklyOffDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WeeklyOffPresentDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LateDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EarlyOutDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MissingPunchDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalWorkMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalPayableOtMinutes = table.Column<int>(type: "int", nullable: false),
                    NightDutyDays = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsPayrollLocked = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyAttendanceSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyAttendanceSummaries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NightBillRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    CalculationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FixedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NightStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    NightEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    MinimumNightWorkMinutes = table.Column<int>(type: "int", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NightBillRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NightBillRules_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NightBillRules_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeApprovalLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyAttendanceId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RawOvertimeMinutes = table.Column<int>(type: "int", nullable: false),
                    RuleOvertimeMinutes = table.Column<int>(type: "int", nullable: false),
                    ApprovedOvertimeMinutes = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeApprovalLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OvertimeApprovalLogs_DailyAttendances_DailyAttendanceId",
                        column: x => x.DailyAttendanceId,
                        principalTable: "DailyAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OvertimeApprovalLogs_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalaryYear = table.Column<int>(type: "int", nullable: false),
                    SalaryMonth = table.Column<int>(type: "int", nullable: false),
                    PayrollType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RunStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeneratedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollRuns_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ErrorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecialDayBillRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    CalculationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FixedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Multiplier = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumWorkMinutes = table.Column<int>(type: "int", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialDayBillRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialDayBillRules_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecialDayBillRules_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceSyncLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceMasterId = table.Column<int>(type: "int", nullable: false),
                    SyncStartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncEndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    NewRecords = table.Column<int>(type: "int", nullable: false),
                    DuplicateRecords = table.Column<int>(type: "int", nullable: false),
                    FailedRecords = table.Column<int>(type: "int", nullable: false),
                    SyncStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSyncLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceSyncLogs_DeviceMasters_DeviceMasterId",
                        column: x => x.DeviceMasterId,
                        principalTable: "DeviceMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanInstallments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    InstallmentYear = table.Column<int>(type: "int", nullable: false),
                    InstallmentMonth = table.Column<int>(type: "int", nullable: false),
                    InstallmentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InstallmentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanInstallments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBonusEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    MonthlyAttendanceSummaryId = table.Column<int>(type: "int", nullable: true),
                    AttendanceBonusRuleId = table.Column<int>(type: "int", nullable: true),
                    BonusType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BonusYear = table.Column<int>(type: "int", nullable: false),
                    BonusMonth = table.Column<int>(type: "int", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceReferenceId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EligibilityStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotEligibleReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntryStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPayrollLocked = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBonusEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeBonusEntries_AttendanceBonusRules_AttendanceBonusRuleId",
                        column: x => x.AttendanceBonusRuleId,
                        principalTable: "AttendanceBonusRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeBonusEntries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeBonusEntries_MonthlyAttendanceSummaries_MonthlyAttendanceSummaryId",
                        column: x => x.MonthlyAttendanceSummaryId,
                        principalTable: "MonthlyAttendanceSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollRunDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollRunId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SalaryStructureId = table.Column<int>(type: "int", nullable: true),
                    MonthlyAttendanceSummaryId = table.Column<int>(type: "int", nullable: true),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OvertimeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NightBillAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HolidayBillAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WeeklyOffBillAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AttendanceBonusAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AdvanceDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetPayable = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DetailStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollRunDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollRunDetails_EmployeeSalaryStructures_SalaryStructureId",
                        column: x => x.SalaryStructureId,
                        principalTable: "EmployeeSalaryStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollRunDetails_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollRunDetails_MonthlyAttendanceSummaries_MonthlyAttendanceSummaryId",
                        column: x => x.MonthlyAttendanceSummaryId,
                        principalTable: "MonthlyAttendanceSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollRunDetails_PayrollRuns_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalTable: "PayrollRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payslips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollRunDetailId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SalaryYear = table.Column<int>(type: "int", nullable: false),
                    SalaryMonth = table.Column<int>(type: "int", nullable: false),
                    NetPayable = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PayslipNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payslips_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payslips_PayrollRunDetails_PayrollRunDetailId",
                        column: x => x.PayrollRunDetailId,
                        principalTable: "PayrollRunDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_CompanyId_ShiftCode",
                table: "Shifts",
                columns: new[] { "CompanyId", "ShiftCode" },
                unique: true,
                filter: "[ShiftCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PunchRecords_DeviceMasterId",
                table: "PunchRecords",
                column: "DeviceMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PunchRecords_EmployeeDeviceMappingId",
                table: "PunchRecords",
                column: "EmployeeDeviceMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDetails_MonthlyAttendanceSummaryId",
                table: "PayrollDetails",
                column: "MonthlyAttendanceSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDetails_SalaryStructureId",
                table: "PayrollDetails",
                column: "SalaryStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_CompanyId_LeaveCode",
                table: "LeaveTypes",
                columns: new[] { "CompanyId", "LeaveCode" },
                unique: true,
                filter: "[LeaveCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendances_ShiftId",
                table: "DailyAttendances",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceApprovalLogs_CompanyId_DailyAttendanceId_ActionAt",
                table: "AttendanceApprovalLogs",
                columns: new[] { "CompanyId", "DailyAttendanceId", "ActionAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceApprovalLogs_DailyAttendanceId",
                table: "AttendanceApprovalLogs",
                column: "DailyAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceBonusRules_CompanyId_RuleName_EffectiveFrom",
                table: "AttendanceBonusRules",
                columns: new[] { "CompanyId", "RuleName", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceBonusRules_DepartmentId",
                table: "AttendanceBonusRules",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceBonusRules_DesignationId",
                table: "AttendanceBonusRules",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceConflicts_CompanyId_EmployeeId_AttendanceDate_ConflictType",
                table: "AttendanceConflicts",
                columns: new[] { "CompanyId", "EmployeeId", "AttendanceDate", "ConflictType" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceConflicts_EmployeeId",
                table: "AttendanceConflicts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CompanyId_TableName_RecordId_ActionAt",
                table: "AuditLogs",
                columns: new[] { "CompanyId", "TableName", "RecordId", "ActionAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendancePunches_CompanyId_DailyAttendanceId_PunchRecordId",
                table: "DailyAttendancePunches",
                columns: new[] { "CompanyId", "DailyAttendanceId", "PunchRecordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendancePunches_DailyAttendanceId",
                table: "DailyAttendancePunches",
                column: "DailyAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendancePunches_PunchRecordId",
                table: "DailyAttendancePunches",
                column: "PunchRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceMasters_CompanyId_DeviceCode",
                table: "DeviceMasters",
                columns: new[] { "CompanyId", "DeviceCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSyncLogs_CompanyId_DeviceMasterId_SyncStartedAt",
                table: "DeviceSyncLogs",
                columns: new[] { "CompanyId", "DeviceMasterId", "SyncStartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSyncLogs_DeviceMasterId",
                table: "DeviceSyncLogs",
                column: "DeviceMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAddresses_CompanyId_EmployeeId_AddressType",
                table: "EmployeeAddresses",
                columns: new[] { "CompanyId", "EmployeeId", "AddressType" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAddresses_EmployeeId",
                table: "EmployeeAddresses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBillEntries_CompanyId_EmployeeId_BillYear_BillMonth_BillType",
                table: "EmployeeBillEntries",
                columns: new[] { "CompanyId", "EmployeeId", "BillYear", "BillMonth", "BillType" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBillEntries_DailyAttendanceId",
                table: "EmployeeBillEntries",
                column: "DailyAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBillEntries_EmployeeId",
                table: "EmployeeBillEntries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBonusEntries_AttendanceBonusRuleId",
                table: "EmployeeBonusEntries",
                column: "AttendanceBonusRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBonusEntries_CompanyId_EmployeeId_BonusYear_BonusMonth_BonusType",
                table: "EmployeeBonusEntries",
                columns: new[] { "CompanyId", "EmployeeId", "BonusYear", "BonusMonth", "BonusType" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBonusEntries_EmployeeId",
                table: "EmployeeBonusEntries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBonusEntries_MonthlyAttendanceSummaryId",
                table: "EmployeeBonusEntries",
                column: "MonthlyAttendanceSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDeviceMappings_CompanyId_DeviceId_DeviceUserId",
                table: "EmployeeDeviceMappings",
                columns: new[] { "CompanyId", "DeviceId", "DeviceUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDeviceMappings_CompanyId_EmployeeId_PunchCardNo",
                table: "EmployeeDeviceMappings",
                columns: new[] { "CompanyId", "EmployeeId", "PunchCardNo" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDeviceMappings_EmployeeId",
                table: "EmployeeDeviceMappings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeJobHistories_CompanyId_EmployeeId_EffectiveFrom",
                table: "EmployeeJobHistories",
                columns: new[] { "CompanyId", "EmployeeId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeJobHistories_DepartmentId",
                table: "EmployeeJobHistories",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeJobHistories_DesignationId",
                table: "EmployeeJobHistories",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeJobHistories_EmployeeId",
                table: "EmployeeJobHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeJobHistories_LineId",
                table: "EmployeeJobHistories",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeJobHistories_SectionId",
                table: "EmployeeJobHistories",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryStructures_CompanyId_EmployeeId_EffectiveFrom",
                table: "EmployeeSalaryStructures",
                columns: new[] { "CompanyId", "EmployeeId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryStructures_EmployeeId",
                table: "EmployeeSalaryStructures",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeWeeklyOffs_CompanyId_EmployeeId_DayOfWeek_EffectiveFrom",
                table: "EmployeeWeeklyOffs",
                columns: new[] { "CompanyId", "EmployeeId", "DayOfWeek", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeWeeklyOffs_EmployeeId",
                table: "EmployeeWeeklyOffs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApprovalLogs_CompanyId_LeaveApplicationId_ApprovalLevel",
                table: "LeaveApprovalLogs",
                columns: new[] { "CompanyId", "LeaveApplicationId", "ApprovalLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApprovalLogs_LeaveApplicationId",
                table: "LeaveApprovalLogs",
                column: "LeaveApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalanceTransactions_CompanyId_EmployeeId_LeaveTypeId_TransactionDate",
                table: "LeaveBalanceTransactions",
                columns: new[] { "CompanyId", "EmployeeId", "LeaveTypeId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalanceTransactions_EmployeeId",
                table: "LeaveBalanceTransactions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalanceTransactions_LeaveApplicationId",
                table: "LeaveBalanceTransactions",
                column: "LeaveApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalanceTransactions_LeaveTypeId",
                table: "LeaveBalanceTransactions",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveConflicts_CompanyId_EmployeeId_ConflictDate",
                table: "LeaveConflicts",
                columns: new[] { "CompanyId", "EmployeeId", "ConflictDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveConflicts_EmployeeId",
                table: "LeaveConflicts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveConflicts_LeaveApplicationId",
                table: "LeaveConflicts",
                column: "LeaveApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicies_CompanyId_LeaveTypeId_EffectiveFrom",
                table: "LeavePolicies",
                columns: new[] { "CompanyId", "LeaveTypeId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicies_LeaveTypeId",
                table: "LeavePolicies",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanInstallments_CompanyId_LoanId_InstallmentYear_InstallmentMonth",
                table: "LoanInstallments",
                columns: new[] { "CompanyId", "LoanId", "InstallmentYear", "InstallmentMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanInstallments_LoanId",
                table: "LoanInstallments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_CompanyId_EmployeeId_LoanStatus",
                table: "Loans",
                columns: new[] { "CompanyId", "EmployeeId", "LoanStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_EmployeeId",
                table: "Loans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPunchRequests_CompanyId_EmployeeId_RequestedPunchTime",
                table: "ManualPunchRequests",
                columns: new[] { "CompanyId", "EmployeeId", "RequestedPunchTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualPunchRequests_CompanyId_RequestStatus",
                table: "ManualPunchRequests",
                columns: new[] { "CompanyId", "RequestStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualPunchRequests_CreatedPunchRecordId",
                table: "ManualPunchRequests",
                column: "CreatedPunchRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPunchRequests_EmployeeId",
                table: "ManualPunchRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAttendanceSummaries_CompanyId_EmployeeId_SalaryYear_SalaryMonth",
                table: "MonthlyAttendanceSummaries",
                columns: new[] { "CompanyId", "EmployeeId", "SalaryYear", "SalaryMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAttendanceSummaries_EmployeeId",
                table: "MonthlyAttendanceSummaries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NightBillRules_CompanyId_DepartmentId_DesignationId_EffectiveFrom",
                table: "NightBillRules",
                columns: new[] { "CompanyId", "DepartmentId", "DesignationId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_NightBillRules_DepartmentId",
                table: "NightBillRules",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NightBillRules_DesignationId",
                table: "NightBillRules",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeApprovalLogs_CompanyId_EmployeeId_ActionAt",
                table: "OvertimeApprovalLogs",
                columns: new[] { "CompanyId", "EmployeeId", "ActionAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeApprovalLogs_DailyAttendanceId",
                table: "OvertimeApprovalLogs",
                column: "DailyAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeApprovalLogs_EmployeeId",
                table: "OvertimeApprovalLogs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRunDetails_CompanyId_PayrollRunId_EmployeeId",
                table: "PayrollRunDetails",
                columns: new[] { "CompanyId", "PayrollRunId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRunDetails_EmployeeId",
                table: "PayrollRunDetails",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRunDetails_MonthlyAttendanceSummaryId",
                table: "PayrollRunDetails",
                column: "MonthlyAttendanceSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRunDetails_PayrollRunId",
                table: "PayrollRunDetails",
                column: "PayrollRunId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRunDetails_SalaryStructureId",
                table: "PayrollRunDetails",
                column: "SalaryStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRuns_CompanyId_SalaryYear_SalaryMonth_PayrollType",
                table: "PayrollRuns",
                columns: new[] { "CompanyId", "SalaryYear", "SalaryMonth", "PayrollType" });

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_CompanyId_PayslipNo",
                table: "Payslips",
                columns: new[] { "CompanyId", "PayslipNo" },
                unique: true,
                filter: "[PayslipNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_EmployeeId",
                table: "Payslips",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_PayrollRunDetailId",
                table: "Payslips",
                column: "PayrollRunDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceErrorLogs_CompanyId_ServiceName_CreatedAt",
                table: "ServiceErrorLogs",
                columns: new[] { "CompanyId", "ServiceName", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialDayBillRules_CompanyId_BillType_DepartmentId_DesignationId_EffectiveFrom",
                table: "SpecialDayBillRules",
                columns: new[] { "CompanyId", "BillType", "DepartmentId", "DesignationId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialDayBillRules_DepartmentId",
                table: "SpecialDayBillRules",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialDayBillRules_DesignationId",
                table: "SpecialDayBillRules",
                column: "DesignationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyAttendances_Shifts_ShiftId",
                table: "DailyAttendances",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDetails_EmployeeSalaryStructures_SalaryStructureId",
                table: "PayrollDetails",
                column: "SalaryStructureId",
                principalTable: "EmployeeSalaryStructures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDetails_MonthlyAttendanceSummaries_MonthlyAttendanceSummaryId",
                table: "PayrollDetails",
                column: "MonthlyAttendanceSummaryId",
                principalTable: "MonthlyAttendanceSummaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PunchRecords_DeviceMasters_DeviceMasterId",
                table: "PunchRecords",
                column: "DeviceMasterId",
                principalTable: "DeviceMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PunchRecords_EmployeeDeviceMappings_EmployeeDeviceMappingId",
                table: "PunchRecords",
                column: "EmployeeDeviceMappingId",
                principalTable: "EmployeeDeviceMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyAttendances_Shifts_ShiftId",
                table: "DailyAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDetails_EmployeeSalaryStructures_SalaryStructureId",
                table: "PayrollDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDetails_MonthlyAttendanceSummaries_MonthlyAttendanceSummaryId",
                table: "PayrollDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PunchRecords_DeviceMasters_DeviceMasterId",
                table: "PunchRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_PunchRecords_EmployeeDeviceMappings_EmployeeDeviceMappingId",
                table: "PunchRecords");

            migrationBuilder.DropTable(
                name: "AttendanceApprovalLogs");

            migrationBuilder.DropTable(
                name: "AttendanceConflicts");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "DailyAttendancePunches");

            migrationBuilder.DropTable(
                name: "DeviceSyncLogs");

            migrationBuilder.DropTable(
                name: "EmployeeAddresses");

            migrationBuilder.DropTable(
                name: "EmployeeBillEntries");

            migrationBuilder.DropTable(
                name: "EmployeeBonusEntries");

            migrationBuilder.DropTable(
                name: "EmployeeDeviceMappings");

            migrationBuilder.DropTable(
                name: "EmployeeJobHistories");

            migrationBuilder.DropTable(
                name: "EmployeeWeeklyOffs");

            migrationBuilder.DropTable(
                name: "LeaveApprovalLogs");

            migrationBuilder.DropTable(
                name: "LeaveBalanceTransactions");

            migrationBuilder.DropTable(
                name: "LeaveConflicts");

            migrationBuilder.DropTable(
                name: "LeavePolicies");

            migrationBuilder.DropTable(
                name: "LoanInstallments");

            migrationBuilder.DropTable(
                name: "ManualPunchRequests");

            migrationBuilder.DropTable(
                name: "NightBillRules");

            migrationBuilder.DropTable(
                name: "OvertimeApprovalLogs");

            migrationBuilder.DropTable(
                name: "Payslips");

            migrationBuilder.DropTable(
                name: "ServiceErrorLogs");

            migrationBuilder.DropTable(
                name: "SpecialDayBillRules");

            migrationBuilder.DropTable(
                name: "DeviceMasters");

            migrationBuilder.DropTable(
                name: "AttendanceBonusRules");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "PayrollRunDetails");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryStructures");

            migrationBuilder.DropTable(
                name: "MonthlyAttendanceSummaries");

            migrationBuilder.DropTable(
                name: "PayrollRuns");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_CompanyId_ShiftCode",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_PunchRecords_DeviceMasterId",
                table: "PunchRecords");

            migrationBuilder.DropIndex(
                name: "IX_PunchRecords_EmployeeDeviceMappingId",
                table: "PunchRecords");

            migrationBuilder.DropIndex(
                name: "IX_PayrollDetails_MonthlyAttendanceSummaryId",
                table: "PayrollDetails");

            migrationBuilder.DropIndex(
                name: "IX_PayrollDetails_SalaryStructureId",
                table: "PayrollDetails");

            migrationBuilder.DropIndex(
                name: "IX_LeaveTypes_CompanyId_LeaveCode",
                table: "LeaveTypes");

            migrationBuilder.DropIndex(
                name: "IX_DailyAttendances_ShiftId",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "BreakMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IsNightShift",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "MinimumOtMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "MinimumWorkMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "PunchAfterMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ShiftCode",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "DeviceMasterId",
                table: "PunchRecords");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "PunchRecords");

            migrationBuilder.DropColumn(
                name: "EmployeeDeviceMappingId",
                table: "PunchRecords");

            migrationBuilder.DropColumn(
                name: "IsDuplicate",
                table: "PunchRecords");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "PunchRecords");

            migrationBuilder.DropColumn(
                name: "PunchCardNo",
                table: "PunchRecords");

            migrationBuilder.DropColumn(
                name: "RawPayload",
                table: "PunchRecords");

            migrationBuilder.DropColumn(
                name: "VerifyType",
                table: "PunchRecords");

            migrationBuilder.DropColumn(
                name: "AttendanceBonusAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "DetailStatus",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "LoanDeduction",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "MonthlyAttendanceSummaryId",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "SalaryStructureId",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "WeeklyOffBillAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "IsCarryForwardAllowed",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "IsEncashmentAllowed",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "IsHalfDayAllowed",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "LeaveCode",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "AdjustedDays",
                table: "LeaveBalances");

            migrationBuilder.DropColumn(
                name: "EarnedDays",
                table: "LeaveBalances");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "LeaveBalances");

            migrationBuilder.DropColumn(
                name: "OpeningBalance",
                table: "LeaveBalances");

            migrationBuilder.DropColumn(
                name: "AppliedAt",
                table: "LeaveApplications");

            migrationBuilder.DropColumn(
                name: "AppliedBy",
                table: "LeaveApplications");

            migrationBuilder.DropColumn(
                name: "CurrentApprovalLevel",
                table: "LeaveApplications");

            migrationBuilder.DropColumn(
                name: "FinalApprovedAt",
                table: "LeaveApplications");

            migrationBuilder.DropColumn(
                name: "FinalApprovedBy",
                table: "LeaveApplications");

            migrationBuilder.DropColumn(
                name: "LeaveDurationType",
                table: "LeaveApplications");

            migrationBuilder.DropColumn(
                name: "EmployeeCategory",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmploymentType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ResignDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ApprovedOvertimeMinutes",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "AttendanceStatus",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "BreakMinutes",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "EarlyOutMinutes",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsEarlyOut",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsHolidayPresent",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsLate",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsLeave",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsNightDuty",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsPaidLeave",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsPresent",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsWeeklyOff",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "IsWeeklyOffPresent",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "NightWorkMinutes",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "PayableDayValue",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "PayableOvertimeMinutes",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "RawOvertimeMinutes",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "RuleOvertimeMinutes",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "ShiftEndDateTime",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "ShiftStartDateTime",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "WindowEndDateTime",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "WindowStartDateTime",
                table: "DailyAttendances");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_CompanyId",
                table: "Shifts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_CompanyId",
                table: "LeaveTypes",
                column: "CompanyId");
        }
    }
}
