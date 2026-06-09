using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Web.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdvancedFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "SavedFilters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FilterJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_SavedFilters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PunchRecords_CompanyId_EmployeeId_PunchTime",
                table: "PunchRecords",
                columns: new[] { "CompanyId", "EmployeeId", "PunchTime" });

            migrationBuilder.CreateIndex(
                name: "IX_PunchRecords_CompanyId_PunchTime",
                table: "PunchRecords",
                columns: new[] { "CompanyId", "PunchTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId_DepartmentId",
                table: "Employees",
                columns: new[] { "CompanyId", "DepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId_FullName",
                table: "Employees",
                columns: new[] { "CompanyId", "FullName" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId_SectionId",
                table: "Employees",
                columns: new[] { "CompanyId", "SectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId_Status",
                table: "Employees",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendances_CompanyId_AttendanceDate",
                table: "DailyAttendances",
                columns: new[] { "CompanyId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendances_CompanyId_IsAbsent_AttendanceDate",
                table: "DailyAttendances",
                columns: new[] { "CompanyId", "IsAbsent", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedFilters_CompanyId_Module",
                table: "SavedFilters",
                columns: new[] { "CompanyId", "Module" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedFilters_CompanyId_UserId_Module_Name",
                table: "SavedFilters",
                columns: new[] { "CompanyId", "UserId", "Module", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedFilters");

            migrationBuilder.DropIndex(
                name: "IX_PunchRecords_CompanyId_EmployeeId_PunchTime",
                table: "PunchRecords");

            migrationBuilder.DropIndex(
                name: "IX_PunchRecords_CompanyId_PunchTime",
                table: "PunchRecords");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyId_DepartmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyId_FullName",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyId_SectionId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyId_Status",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_DailyAttendances_CompanyId_AttendanceDate",
                table: "DailyAttendances");

            migrationBuilder.DropIndex(
                name: "IX_DailyAttendances_CompanyId_IsAbsent_AttendanceDate",
                table: "DailyAttendances");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
