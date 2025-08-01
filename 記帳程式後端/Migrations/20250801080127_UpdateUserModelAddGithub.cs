using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 記帳程式後端.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserModelAddGithub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GithubId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GithubId",
                table: "Users");
        }
    }
}
