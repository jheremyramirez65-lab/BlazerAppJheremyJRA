using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAppJheremyJRA.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCategorias : Migration
    {
        /// <inheritdoc />

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Nombre = table.Column<string>(
                        type: "varchar(100)",
                        unicode: false,
                        maxLength: 100,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Categoria",
                        x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "LibroCategoria",
                columns: table => new
                {
                    IdLibro = table.Column<int>(
                        type: "int",
                        nullable: false),

                    IdCategoria = table.Column<int>(
                        type: "int",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_LibroCategoria",
                        x => new { x.IdLibro, x.IdCategoria });

                    table.ForeignKey(
                        name: "FK_LibroCategoria_Categoria_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categoria",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_LibroCategoria_Libro_IdLibro",
                        column: x => x.IdLibro,
                        principalTable: "Libro",
                        principalColumn: "idLibro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibroCategoria_IdCategoria",
                table: "LibroCategoria",
                column: "IdCategoria");
        }


        /// <inheritdoc />

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LibroCategoria");

            migrationBuilder.DropTable(
                name: "Categoria");
        }

    }
}
