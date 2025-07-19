using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wacdoprojet.Migrations
{
    /// <inheritdoc />
    public partial class RenomeAdministrateurConnectablebis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
            name: "Connectable",
            table: "AspNetUsers",
            type: "tinyint(1)",
            nullable: false,
            defaultValue: false);

            // ⚠️ Optionnel : copier les données de l’ancienne colonne si elle existe (si elle n’a pas été supprimée déjà)
            // migrationBuilder.Sql("UPDATE AspNetUsers SET Connectable = Administrateur");

            // Supprime l’ancienne colonne
            migrationBuilder.DropColumn(
                name: "Administrateur",
                table: "AspNetUsers");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
       name: "Administrateur",
       table: "AspNetUsers",
       type: "tinyint(1)",
       nullable: false,
       defaultValue: false);

            migrationBuilder.DropColumn(
                name: "Connectable",
                table: "AspNetUsers");
        }
    }
}
