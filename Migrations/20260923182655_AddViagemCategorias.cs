using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace travel_ai.Migrations
{
    /// <inheritdoc />
    public partial class AddViagemCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "viagem_categorias",
                columns: table => new
                {
                    ViagemId = table.Column<int>(type: "integer", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_viagem_categorias", x => new { x.ViagemId, x.CategoriaId });
                    table.ForeignKey(
                        name: "FK_viagem_categorias_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_viagem_categorias_viagens_ViagemId",
                        column: x => x.ViagemId,
                        principalTable: "viagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_viagem_categorias_CategoriaId",
                table: "viagem_categorias",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "viagem_categorias");
        }
    }
}
