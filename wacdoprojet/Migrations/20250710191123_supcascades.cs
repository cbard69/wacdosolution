using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wacdoprojet.Migrations
{
    /// <inheritdoc />
    public partial class supcascades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_AspNetUsers_CollaborateurId",
                table: "Affectations");

            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_Postes_PosteId",
                table: "Affectations");

            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_Restaurants_RestaurantId",
                table: "Affectations");

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_AspNetUsers_CollaborateurId",
                table: "Affectations",
                column: "CollaborateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_Postes_PosteId",
                table: "Affectations",
                column: "PosteId",
                principalTable: "Postes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_Restaurants_RestaurantId",
                table: "Affectations",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_AspNetUsers_CollaborateurId",
                table: "Affectations");

            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_Postes_PosteId",
                table: "Affectations");

            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_Restaurants_RestaurantId",
                table: "Affectations");

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_AspNetUsers_CollaborateurId",
                table: "Affectations",
                column: "CollaborateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_Postes_PosteId",
                table: "Affectations",
                column: "PosteId",
                principalTable: "Postes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_Restaurants_RestaurantId",
                table: "Affectations",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
