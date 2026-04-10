using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirtsMigratiions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "USUARIO",
                schema: "Identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CORREO_PRINCIPAL = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    CORREO_VERIFICADO = table.Column<bool>(type: "bit", nullable: false),
                    NOMBRE_USUARIO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NOMBRE_MOSTRAR = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    URL_AVATAR = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ESTADO = table.Column<int>(type: "int", nullable: false),
                    ULTIMO_INICIO_SESION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ES_ADMINISTRADOR_PLATAFORMA = table.Column<bool>(type: "bit", nullable: false),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ACTUALIZACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ELIMINACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ES_ELIMINADO = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CREDENCIAL",
                schema: "Identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TIPO = table.Column<int>(type: "int", nullable: false),
                    ESTADO = table.Column<int>(type: "int", nullable: false),
                    HASH_SECRETO = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ALGORITMO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FECHA_EXPIRACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ULTIMO_USO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ACTUALIZACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ELIMINACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ES_ELIMINADO = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CREDENCIAL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CREDENCIAL_USUARIO",
                        column: x => x.USUARIO_ID,
                        principalSchema: "Identity",
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IDENTIDAD_EXTERNA",
                schema: "Identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TIPO_PROVEEDOR = table.Column<int>(type: "int", nullable: false),
                    SUJETO_PROVEEDOR = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CORREO_PROVEEDOR = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    CORREO_PROVEEDOR_VERIFICADO = table.Column<bool>(type: "bit", nullable: false),
                    FECHA_VINCULACION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ULTIMO_INICIO_SESION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ACTUALIZACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ELIMINACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ES_ELIMINADO = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IDENTIDAD_EXTERNA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_IDENTIDAD_EXTERNA_USUARIO",
                        column: x => x.USUARIO_ID,
                        principalSchema: "Identity",
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MEMBRESIA_TENANT",
                schema: "Identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TENANT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CODIGO_ROL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ESTADO = table.Column<int>(type: "int", nullable: false),
                    FECHA_VINCULACION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ACTUALIZACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ELIMINACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ES_ELIMINADO = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEMBRESIA_TENANT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MEMBRESIA_TENANT_USUARIO",
                        column: x => x.USUARIO_ID,
                        principalSchema: "Identity",
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SESION",
                schema: "Identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TENANT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    METODO_AUTENTICACION = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DIRECCION_IP = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    USER_AGENT = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NOMBRE_DISPOSITIVO = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ESTADO = table.Column<int>(type: "int", nullable: false),
                    FECHA_INICIO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ULTIMA_ACTIVIDAD = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FECHA_EXPIRACION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FECHA_REVOCACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ACTUALIZACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ELIMINACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ES_ELIMINADO = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SESION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SESION_USUARIO",
                        column: x => x.USUARIO_ID,
                        principalSchema: "Identity",
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TOKEN_VERIFICACION",
                schema: "Identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TIPO = table.Column<int>(type: "int", nullable: false),
                    HASH_TOKEN = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FECHA_EXPIRACION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FECHA_USO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ACTUALIZACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ELIMINACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ES_ELIMINADO = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TOKEN_VERIFICACION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TOKEN_VERIFICACION_USUARIO",
                        column: x => x.USUARIO_ID,
                        principalSchema: "Identity",
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TOKEN_REFRESH",
                schema: "Identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SESION_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TENANT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HASH_TOKEN = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FECHA_EMISION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FECHA_EXPIRACION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FECHA_REVOCACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FECHA_REUSO_DETECTADO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TOKEN_ROTADO_DESDE_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    REEMPLAZADO_POR_TOKEN_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ACTUALIZACION = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FECHA_ELIMINACION = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ES_ELIMINADO = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TOKEN_REFRESH", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TOKEN_REFRESH_REEMPLAZADO_POR",
                        column: x => x.REEMPLAZADO_POR_TOKEN_ID,
                        principalSchema: "Identity",
                        principalTable: "TOKEN_REFRESH",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TOKEN_REFRESH_ROTADO_DESDE",
                        column: x => x.TOKEN_ROTADO_DESDE_ID,
                        principalSchema: "Identity",
                        principalTable: "TOKEN_REFRESH",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TOKEN_REFRESH_SESION",
                        column: x => x.SESION_ID,
                        principalSchema: "Identity",
                        principalTable: "SESION",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TOKEN_REFRESH_USUARIO",
                        column: x => x.USUARIO_ID,
                        principalSchema: "Identity",
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CREDENCIAL_USUARIO_ID",
                schema: "Identity",
                table: "CREDENCIAL",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_IDENTIDAD_EXTERNA_TIPO_PROVEEDOR_SUJETO_PROVEEDOR",
                schema: "Identity",
                table: "IDENTIDAD_EXTERNA",
                columns: new[] { "TIPO_PROVEEDOR", "SUJETO_PROVEEDOR" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IDENTIDAD_EXTERNA_USUARIO_ID",
                schema: "Identity",
                table: "IDENTIDAD_EXTERNA",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MEMBRESIA_TENANT_TENANT_ID",
                schema: "Identity",
                table: "MEMBRESIA_TENANT",
                column: "TENANT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MEMBRESIA_TENANT_USUARIO_ID",
                schema: "Identity",
                table: "MEMBRESIA_TENANT",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MEMBRESIA_TENANT_USUARIO_ID_TENANT_ID",
                schema: "Identity",
                table: "MEMBRESIA_TENANT",
                columns: new[] { "USUARIO_ID", "TENANT_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SESION_ESTADO",
                schema: "Identity",
                table: "SESION",
                column: "ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_SESION_FECHA_EXPIRACION",
                schema: "Identity",
                table: "SESION",
                column: "FECHA_EXPIRACION");

            migrationBuilder.CreateIndex(
                name: "IX_SESION_TENANT_ID",
                schema: "Identity",
                table: "SESION",
                column: "TENANT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SESION_USUARIO_ID",
                schema: "Identity",
                table: "SESION",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_REFRESH_FECHA_EXPIRACION",
                schema: "Identity",
                table: "TOKEN_REFRESH",
                column: "FECHA_EXPIRACION");

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_REFRESH_HASH_TOKEN",
                schema: "Identity",
                table: "TOKEN_REFRESH",
                column: "HASH_TOKEN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_REFRESH_REEMPLAZADO_POR_TOKEN_ID",
                schema: "Identity",
                table: "TOKEN_REFRESH",
                column: "REEMPLAZADO_POR_TOKEN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_REFRESH_SESION_ID",
                schema: "Identity",
                table: "TOKEN_REFRESH",
                column: "SESION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_REFRESH_TOKEN_ROTADO_DESDE_ID",
                schema: "Identity",
                table: "TOKEN_REFRESH",
                column: "TOKEN_ROTADO_DESDE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_REFRESH_USUARIO_ID",
                schema: "Identity",
                table: "TOKEN_REFRESH",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_VERIFICACION_FECHA_EXPIRACION",
                schema: "Identity",
                table: "TOKEN_VERIFICACION",
                column: "FECHA_EXPIRACION");

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_VERIFICACION_HASH_TOKEN",
                schema: "Identity",
                table: "TOKEN_VERIFICACION",
                column: "HASH_TOKEN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TOKEN_VERIFICACION_USUARIO_ID",
                schema: "Identity",
                table: "TOKEN_VERIFICACION",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_CORREO_PRINCIPAL",
                schema: "Identity",
                table: "USUARIO",
                column: "CORREO_PRINCIPAL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_NOMBRE_USUARIO",
                schema: "Identity",
                table: "USUARIO",
                column: "NOMBRE_USUARIO",
                unique: true,
                filter: "[NOMBRE_USUARIO] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CREDENCIAL",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "IDENTIDAD_EXTERNA",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "MEMBRESIA_TENANT",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TOKEN_REFRESH",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TOKEN_VERIFICACION",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "SESION",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "USUARIO",
                schema: "Identity");
        }
    }
}
