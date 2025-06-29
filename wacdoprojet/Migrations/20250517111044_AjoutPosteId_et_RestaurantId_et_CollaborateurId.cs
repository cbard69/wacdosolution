using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wacdoprojet.Migrations
{
    /// <inheritdoc />
    public partial class AjoutPosteId_et_RestaurantId_et_CollaborateurId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_AspNetUsers_CollaborateurId",
                table: "Affectations");

            migrationBuilder.AlterColumn<string>(
                name: "CollaborateurId",
                table: "Affectations",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_AspNetUsers_CollaborateurId",
                table: "Affectations",
                column: "CollaborateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_AspNetUsers_CollaborateurId",
                table: "Affectations");

            migrationBuilder.AlterColumn<string>(
                name: "CollaborateurId",
                table: "Affectations",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_AspNetUsers_CollaborateurId",
                table: "Affectations",
                column: "CollaborateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
