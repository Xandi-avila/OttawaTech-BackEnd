using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ottawa_Tech___BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AlterarClienteAtivoParaSituacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClienteAtivo",
                table: "Clientes",
                newName: "Situacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Situacao",
                table: "Clientes",
                newName: "ClienteAtivo");
        }
    }
}