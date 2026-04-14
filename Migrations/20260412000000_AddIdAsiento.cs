using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Documentos_por_Pagar.Migrations
{
    /// <inheritdoc />
    public partial class AddIdAsiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdAsiento",
                table: "DocumentosPagar",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdAsiento",
                table: "DocumentosPagar");
        }
    }
}
