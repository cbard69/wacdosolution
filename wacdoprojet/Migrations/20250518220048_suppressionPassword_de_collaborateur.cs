using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wacdoprojet.Migrations
{
    /// <inheritdoc />
    public partial class suppressionPassword_de_collaborateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AspNetUsers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
