using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Web.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class HrManpowerSeparation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeSeparations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SeparationType = table.Column<int>(type: "int", nullable: false),
                    SeparationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoticeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_EmployeeSeparations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSeparations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HiringRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    LineId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    RequestedCount = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetJoinDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HiringRequestStatus = table.Column<int>(type: "int", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
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
                    table.PrimaryKey("PK_HiringRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiringRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HiringRequests_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HiringRequests_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HiringRequests_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffingPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    LineId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    RequiredCount = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_StaffingPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffingPlans_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffingPlans_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffingPlans_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffingPlans_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSeparations_CompanyId_EmployeeId",
                table: "EmployeeSeparations",
                columns: new[] { "CompanyId", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSeparations_CompanyId_SeparationDate",
                table: "EmployeeSeparations",
                columns: new[] { "CompanyId", "SeparationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSeparations_EmployeeId",
                table: "EmployeeSeparations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringRequests_CompanyId_HiringRequestStatus",
                table: "HiringRequests",
                columns: new[] { "CompanyId", "HiringRequestStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_HiringRequests_DepartmentId",
                table: "HiringRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringRequests_DesignationId",
                table: "HiringRequests",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringRequests_LineId",
                table: "HiringRequests",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringRequests_SectionId",
                table: "HiringRequests",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffingPlans_CompanyId_DepartmentId_SectionId_LineId_DesignationId_Year_Month",
                table: "StaffingPlans",
                columns: new[] { "CompanyId", "DepartmentId", "SectionId", "LineId", "DesignationId", "Year", "Month" },
                unique: true,
                filter: "[SectionId] IS NOT NULL AND [LineId] IS NOT NULL AND [DesignationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StaffingPlans_DepartmentId",
                table: "StaffingPlans",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffingPlans_DesignationId",
                table: "StaffingPlans",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffingPlans_LineId",
                table: "StaffingPlans",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffingPlans_SectionId",
                table: "StaffingPlans",
                column: "SectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSeparations");

            migrationBuilder.DropTable(
                name: "HiringRequests");

            migrationBuilder.DropTable(
                name: "StaffingPlans");
        }
    }
}
