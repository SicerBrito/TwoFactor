using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistencia.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id_Rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NombreRol = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id_Rol);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id_Usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    twoFactorSecret = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id_Usuario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expires = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuario",
                        principalColumn: "Id_Usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolUsuario",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "int", nullable: false),
                    UsuariosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolUsuario", x => new { x.RolesId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_RolUsuario_Rol_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Rol",
                        principalColumn: "Id_Rol",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolUsuario_Usuario_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuario",
                        principalColumn: "Id_Usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UsuarioRol",
                columns: table => new
                {
                    Id_UsuarioRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Usuario_Id = table.Column<int>(type: "int", nullable: false),
                    Rol_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRol", x => x.Id_UsuarioRol);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Rol_Rol_Id",
                        column: x => x.Rol_Id,
                        principalTable: "Rol",
                        principalColumn: "Id_Rol",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Usuario_Usuario_Id",
                        column: x => x.Usuario_Id,
                        principalTable: "Usuario",
                        principalColumn: "Id_Usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "Id_Rol", "NombreRol" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Gerente" },
                    { 3, "Empleado" }
                });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "Id_Usuario", "createDate", "Email", "Password", "twoFactorSecret", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "britodelgado514@gmail.com", "123456", null, "Sicer Brito" },
                    { 2, new DateTime(2024, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "angedeveloper@gmail.com", "123", null, "Angelica Morales" }
                });

            migrationBuilder.InsertData(
                table: "UsuarioRol",
                columns: new[] { "Id_UsuarioRol", "Rol_Id", "Usuario_Id" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 3, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolUsuario_UsuariosId",
                table: "RolUsuario",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_Username_Email",
                table: "Usuario",
                columns: new[] { "Username", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_Rol_Id",
                table: "UsuarioRol",
                column: "Rol_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_Usuario_Id",
                table: "UsuarioRol",
                column: "Usuario_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RolUsuario");

            migrationBuilder.DropTable(
                name: "UsuarioRol");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
