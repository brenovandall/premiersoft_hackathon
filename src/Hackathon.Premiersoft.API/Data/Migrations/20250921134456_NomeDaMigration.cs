using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Premiersoft.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomeDaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicos_Cidades_MunicipioId",
                table: "Medicos");

            migrationBuilder.RenameColumn(
                name: "MunicipioId",
                table: "Medicos",
                newName: "Codigo_MunicipioId");

            migrationBuilder.RenameIndex(
                name: "IX_Medicos_MunicipioId",
                table: "Medicos",
                newName: "IX_Medicos_Codigo_MunicipioId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAlocacao",
                table: "PatientsHospitals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Distancia",
                table: "PatientsHospitals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "PatientsHospitals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Prioridade",
                table: "PatientsHospitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PatientsHospitals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Cid10Especialidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cid10Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prioridade = table.Column<int>(type: "int", nullable: false),
                    EspecialidadePrimaria = table.Column<bool>(type: "bit", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cid10Especialidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cid10Especialidades_Cid10_Cid10Id",
                        column: x => x.Cid10Id,
                        principalTable: "Cid10",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cid10Especialidades_Cid10Id",
                table: "Cid10Especialidades",
                column: "Cid10Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicos_Cidades_Codigo_MunicipioId",
                table: "Medicos",
                column: "Codigo_MunicipioId",
                principalTable: "Cidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicos_Cidades_Codigo_MunicipioId",
                table: "Medicos");

            migrationBuilder.DropTable(
                name: "Cid10Especialidades");

            migrationBuilder.DropColumn(
                name: "DataAlocacao",
                table: "PatientsHospitals");

            migrationBuilder.DropColumn(
                name: "Distancia",
                table: "PatientsHospitals");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "PatientsHospitals");

            migrationBuilder.DropColumn(
                name: "Prioridade",
                table: "PatientsHospitals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PatientsHospitals");

            migrationBuilder.RenameColumn(
                name: "Codigo_MunicipioId",
                table: "Medicos",
                newName: "MunicipioId");

            migrationBuilder.RenameIndex(
                name: "IX_Medicos_Codigo_MunicipioId",
                table: "Medicos",
                newName: "IX_Medicos_MunicipioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicos_Cidades_MunicipioId",
                table: "Medicos",
                column: "MunicipioId",
                principalTable: "Cidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
