using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 記帳程式後端.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    picPath1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    picPath2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    smallPicPath1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    smallPicPath2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
