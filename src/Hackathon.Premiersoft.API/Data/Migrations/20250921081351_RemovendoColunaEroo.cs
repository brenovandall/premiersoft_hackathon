using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Premiersoft.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovendoColunaEroo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Erros",
                table: "Cidades");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Erros",
                table: "Cidades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
