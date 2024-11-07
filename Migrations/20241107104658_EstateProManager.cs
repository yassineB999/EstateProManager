using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstateProManager.Migrations
{
    /// <inheritdoc />
    public partial class EstateProManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proprietes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Taille = table.Column<long>(type: "bigint", nullable: false),
                    Emplacement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caracteristiques = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Documents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valeur = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DateDeConstruction = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Propriet__3214EC0736E8C92B", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    libelle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__3214EC07A44F3C13", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotDePasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    NumeroDeTelephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdressePostale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdRole = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Utilisat__3214EC07949A8F51", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Utilisate__IdRol__267ABA7A",
                        column: x => x.IdRole,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contrats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPropriete = table.Column<long>(type: "bigint", nullable: false),
                    IdLocataire = table.Column<long>(type: "bigint", nullable: false),
                    DateDebut = table.Column<DateOnly>(type: "date", nullable: false),
                    DateFin = table.Column<DateOnly>(type: "date", nullable: false),
                    PaiementMensuel = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Contrats__3214EC07447F1DB6", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Contrats__IdLoca__2C3393D0",
                        column: x => x.IdLocataire,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Contrats__IdProp__2B3F6F97",
                        column: x => x.IdPropriete,
                        principalTable: "Proprietes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Demandes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPropriete = table.Column<long>(type: "bigint", nullable: false),
                    IdUtilisateur = table.Column<long>(type: "bigint", nullable: false),
                    TypeDemande = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailsDemande = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Demandes__3214EC07D33E4B29", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Demandes__IdProp__33D4B598",
                        column: x => x.IdPropriete,
                        principalTable: "Proprietes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Demandes__IdUtil__34C8D9D1",
                        column: x => x.IdUtilisateur,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ventes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPropriete = table.Column<long>(type: "bigint", nullable: false),
                    IdAcheteur = table.Column<long>(type: "bigint", nullable: false),
                    IdVendeur = table.Column<long>(type: "bigint", nullable: false),
                    PrixVente = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DateVente = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ventes__3214EC077DD9863D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Ventes__IdAchete__300424B4",
                        column: x => x.IdAcheteur,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Ventes__IdPropri__2F10007B",
                        column: x => x.IdPropriete,
                        principalTable: "Proprietes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Ventes__IdVendeu__30F848ED",
                        column: x => x.IdVendeur,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Taches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDemande = table.Column<long>(type: "bigint", nullable: false),
                    IdPrestataire = table.Column<long>(type: "bigint", nullable: false),
                    DetailsTache = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatutTache = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Taches__3214EC076FB6A2E7", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Taches__IdDemand__37A5467C",
                        column: x => x.IdDemande,
                        principalTable: "Demandes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Taches__IdPresta__38996AB5",
                        column: x => x.IdPrestataire,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contrats_IdLocataire",
                table: "Contrats",
                column: "IdLocataire");

            migrationBuilder.CreateIndex(
                name: "IX_Contrats_IdPropriete",
                table: "Contrats",
                column: "IdPropriete");

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_IdPropriete",
                table: "Demandes",
                column: "IdPropriete");

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_IdUtilisateur",
                table: "Demandes",
                column: "IdUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Taches_IdDemande",
                table: "Taches",
                column: "IdDemande");

            migrationBuilder.CreateIndex(
                name: "IX_Taches_IdPrestataire",
                table: "Taches",
                column: "IdPrestataire");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_IdRole",
                table: "Utilisateurs",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_Ventes_IdAcheteur",
                table: "Ventes",
                column: "IdAcheteur");

            migrationBuilder.CreateIndex(
                name: "IX_Ventes_IdPropriete",
                table: "Ventes",
                column: "IdPropriete");

            migrationBuilder.CreateIndex(
                name: "IX_Ventes_IdVendeur",
                table: "Ventes",
                column: "IdVendeur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contrats");

            migrationBuilder.DropTable(
                name: "Taches");

            migrationBuilder.DropTable(
                name: "Ventes");

            migrationBuilder.DropTable(
                name: "Demandes");

            migrationBuilder.DropTable(
                name: "Proprietes");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
