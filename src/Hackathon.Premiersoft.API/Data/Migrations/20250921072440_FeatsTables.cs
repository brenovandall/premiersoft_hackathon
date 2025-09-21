using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Premiersoft.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class FeatsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Especialidades",
                table: "Hospitais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DoctorsHospitals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HospitalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorsHospitals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorsHospitals_Hospitais_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitais",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DoctorsHospitals_Medicos_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Medicos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientsHospitals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HospitalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientsHospitals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientsHospitals_Hospitais_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitais",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PatientsHospitals_Pacientes_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorsHospitals_DoctorId",
                table: "DoctorsHospitals",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorsHospitals_HospitalId",
                table: "DoctorsHospitals",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientsHospitals_HospitalId",
                table: "PatientsHospitals",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientsHospitals_PatientId",
                table: "PatientsHospitals",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorsHospitals");

            migrationBuilder.DropTable(
                name: "PatientsHospitals");

            migrationBuilder.DropColumn(
                name: "Especialidades",
                table: "Hospitais");
        }
    }
}
