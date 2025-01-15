using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityMVCCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__Employee__3213E83F1BCE2A1F",
                table: "Employee");

            migrationBuilder.AddPrimaryKey(
                name: "id",
                table: "Employee",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "id",
                table: "Employee");

            migrationBuilder.AddPrimaryKey(
                name: "EmployeeId",
                table: "Employee",
                column: "id");
        }
    }
}
