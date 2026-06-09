using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Web.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DesignationSectionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Designations",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE d
                SET d.SectionId = s.Id
                FROM Designations d
                CROSS APPLY (
                    SELECT TOP 1 sec.Id
                    FROM Sections sec
                    WHERE sec.CompanyId = d.CompanyId AND sec.IsDeleted = 0
                    ORDER BY sec.Id
                ) s
                WHERE d.SectionId IS NULL
                """);

            migrationBuilder.AlterColumn<int>(
                name: "SectionId",
                table: "Designations",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designations_SectionId",
                table: "Designations",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designations_Sections_SectionId",
                table: "Designations",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designations_Sections_SectionId",
                table: "Designations");

            migrationBuilder.DropIndex(
                name: "IX_Designations_SectionId",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Designations");
        }
    }
}
