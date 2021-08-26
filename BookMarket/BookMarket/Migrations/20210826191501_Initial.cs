using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookMarket.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "BookUtenti",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(maxLength: 30, nullable: false),
                    Cognome = table.Column<string>(maxLength: 50, nullable: false),
                    Mail = table.Column<string>(maxLength: 200, nullable: false),
                    Password = table.Column<string>(maxLength: 2000, nullable: false),
                    Token = table.Column<string>(nullable: false),
                    Ip = table.Column<string>(maxLength: 200, nullable: false),
                    AppuntamentoRilascio = table.Column<DateTime>(nullable: true),
                    AppuntamentoRitiro = table.Column<DateTime>(nullable: true),
                    MailToken = table.Column<string>(nullable: true),
                    LastMailSent = table.Column<DateTime>(nullable: true),
                    AppuntamentoFinale = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookUtenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventsLog",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Evento = table.Column<string>(maxLength: 3000, nullable: false),
                    Data = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookLibri",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(maxLength: 200, nullable: false),
                    Codice = table.Column<string>(maxLength: 13, nullable: false),
                    Materia = table.Column<string>(maxLength: 100, nullable: false),
                    IdProprietario = table.Column<int>(nullable: false),
                    Prezzo = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IdAcquirente = table.Column<int>(nullable: true),
                    DataCaricamento = table.Column<DateTime>(nullable: false),
                    Venduto = table.Column<bool>(nullable: true),
                    IdUtente = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookLibri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookLibri_BookUtenti_IdAcquirente",
                        column: x => x.IdAcquirente,
                        principalSchema: "dbo",
                        principalTable: "BookUtenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookLibri_BookUtenti_IdProprietario",
                        column: x => x.IdProprietario,
                        principalSchema: "dbo",
                        principalTable: "BookUtenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookCarrello",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtente = table.Column<int>(nullable: true),
                    IdLibro = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCarrello", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookCarrello_BookLibri_IdLibro",
                        column: x => x.IdLibro,
                        principalSchema: "dbo",
                        principalTable: "BookLibri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCarrello_BookUtenti_IdUtente",
                        column: x => x.IdUtente,
                        principalSchema: "dbo",
                        principalTable: "BookUtenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCarrello_IdLibro",
                schema: "dbo",
                table: "BookCarrello",
                column: "IdLibro");

            migrationBuilder.CreateIndex(
                name: "IX_BookCarrello_IdUtente",
                schema: "dbo",
                table: "BookCarrello",
                column: "IdUtente");

            migrationBuilder.CreateIndex(
                name: "IX_BookLibri_IdAcquirente",
                schema: "dbo",
                table: "BookLibri",
                column: "IdAcquirente");

            migrationBuilder.CreateIndex(
                name: "IX_BookLibri_IdProprietario",
                schema: "dbo",
                table: "BookLibri",
                column: "IdProprietario");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookCarrello",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EventsLog",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BookLibri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BookUtenti",
                schema: "dbo");
        }
    }
}
