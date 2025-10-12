using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class FixFieldNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reason_id",
                table: "Expenses",
                newName: "subcategoryId");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "Expenses",
                newName: "categoryId");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "Expenses",
                newName: "accountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "subcategoryId",
                table: "Expenses",
                newName: "reason_id");

            migrationBuilder.RenameColumn(
                name: "categoryId",
                table: "Expenses",
                newName: "category_id");

            migrationBuilder.RenameColumn(
                name: "accountId",
                table: "Expenses",
                newName: "account_id");
        }
    }
}
